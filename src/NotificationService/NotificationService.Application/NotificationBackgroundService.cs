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

public class NotificationBackgroundService : BackgroundService
{
    private readonly ILogger<NotificationBackgroundService> _logger;
    private readonly EventBus _eventBus;

    
    private readonly IServiceScopeFactory _scopeFactory;

    public NotificationBackgroundService(ILogger<NotificationBackgroundService> logger, EventBus eventBus,
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
        await _eventBus.SubscribeAsync<OrderConfirmedEvent>(
            queueName: RabbitMqConstants.OrderConfirmedQueue,
            routingKey: RabbitMqConstants.OrderConfirmedRoutingKey,
            onMessage: async (orderEvent) =>
            {
                _logger.LogInformation($"Received OrderCreatedEvent for Order Id: {orderEvent.OrderId}");
                using (var scope = _scopeFactory.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<NotificationDbContext>>();
                    var notificationRepository = unitOfWork.GetRepository<INotificationRepository>();
                    var notification = new Notification
                    {
                        Id = Guid.CreateVersion7(),
                        CustomerId = orderEvent.CustomerId,
                        NotificationType = NotificationType.OrderConfirmed,
                        Message = "Your order has been confirmed.",
                        Status = NotificationStatus.Pending
                    };
                    await notificationRepository.AddAsync(notification);
                    await unitOfWork.SaveChangesAsync();
                    
                    var notificationCreatedEvent = new NotificationCreatedEvent
                    {
                        Id = notification.Id,
                        UserId = notification.CustomerId,
                        Message = notification.Message,
                        NotificationType = notification.NotificationType.ToString(),
                        Status = notification.Status.ToString()
                    };
        
                    var retryPolicy = Policy
                        .Handle<MessaagingException>()
                        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
                    await retryPolicy.ExecuteAsync(async () =>

                        _eventBus.PublishAsync(notificationCreatedEvent, RabbitMqConstants.NotificationRoutingKey,
                            RabbitMqConstants.NotificationQueue));

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