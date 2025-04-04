using Common.Exceptions;
using Common.Infrastructure;
using Common.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Events;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Data;
using Polly;

namespace NotificationService.Application;

public class SendNotificationBackgroundService : BackgroundService
{
    private readonly ILogger<NotificationBackgroundService> _logger;
    private readonly EventBus _eventBus;

    
    private readonly IServiceScopeFactory _scopeFactory;

    public SendNotificationBackgroundService(ILogger<NotificationBackgroundService> logger, EventBus eventBus,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _eventBus = eventBus;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Stock background service is starting.");

        // Subscribe to the OrderCreatedEvent using the EventBus.
        await _eventBus.SubscribeAsync<NotificationCreatedEvent>(
            queueName: RabbitMqConstants.NotificationQueue,
            routingKey: RabbitMqConstants.NotificationRoutingKey,
            onMessage: async (notificationCreatedEvent) =>
            {
                _logger.LogInformation($"Received NotificationCreatedEvent for Id: {notificationCreatedEvent.Id}");
                using (var scope = _scopeFactory.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<NotificationDbContext>>();
                    var notificationRepository = unitOfWork.GetRepository<INotificationRepository>();
                    var notification = await notificationRepository.GetByIdAsync(notificationCreatedEvent.Id);
                    //TODO: send notification in here
                    
                    notification.UpdatedAt = DateTime.UtcNow;
                    notification.Status = NotificationStatus.Sent;
                    notificationRepository.Update(notification);
                    await unitOfWork.SaveChangesAsync();
                    
                    var notificationSentEvent = new NotificationSentEvent()
                    {
                        Id = notification.Id,
                        UserId = notification.CustomerId,
                        Message = notification.Message,
                        NotificationType = notification.NotificationType.ToString(),
                        Status = NotificationStatus.Sent.ToString()
                    };
        
                    var retryPolicy = Policy
                        .Handle<MessaagingException>()
                        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
                    await retryPolicy.ExecuteAsync(async () =>

                        _eventBus.PublishAsync(notificationSentEvent, RabbitMqConstants.NotificationSentRoutingKey,
                            RabbitMqConstants.NotificationSentQueue));

                }

                //check if orderEvent items are exists
                await Task.CompletedTask;
            });

        // Keep the service running until a cancellation is requested.
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        _logger.LogInformation("Stock background service is stopping.");
    }
}