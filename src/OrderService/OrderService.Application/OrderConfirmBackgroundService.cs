using Common.Exceptions;
using Common.Infrastructure;
using Common.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderService.Domain.Entities;
using OrderService.Domain.Events;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Data;
using Polly;

namespace OrderService.Application;

public class OrderConfirmBackgroundService : BackgroundService
{
    private readonly ILogger<OrderConfirmBackgroundService> _logger;
    private readonly EventBus _eventBus;

    
    private readonly IServiceScopeFactory _scopeFactory;

    public OrderConfirmBackgroundService(ILogger<OrderConfirmBackgroundService> logger, EventBus eventBus,
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
        await _eventBus.SubscribeAsync<StockUpdatedEvent>(
            queueName: RabbitMqConstants.StockQueueUpdated,
            routingKey: RabbitMqConstants.StockUpdatedRoutingKey,
            onMessage: async (stockUpdateEvent) =>
            {
                _logger.LogInformation($"Received OrderCreatedEvent for Order Id: {stockUpdateEvent.OrderId}");

                using (var scope = _scopeFactory.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<OrderDbContext>>();
                    var repository = unitOfWork.GetRepository<IOrderRepository>();
                    var order = await repository.GetByIdAsync(stockUpdateEvent.OrderId);
                    order.Status = OrderStatus.Confirmed;
                    repository.Update(order);
                    await unitOfWork.SaveChangesAsync();
                    
                    
                    var retryPolicy = Policy
                        .Handle<MessaagingException>()
                        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        await _eventBus.PublishAsync(new OrderConfirmedEvent()
                        {
                            OrderId = order.Id,
                            Items = order.Items.Select(x=> new OrderItem(){
                                ProductId = x.ProductId,
                                Quantity = x.Quantity,
                            }).ToList(),
                            CustomerId = order.CustomerId,

                        }, RabbitMqConstants.OrderConfirmedRoutingKey, RabbitMqConstants.OrderConfirmedQueue);
                    });
                }

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