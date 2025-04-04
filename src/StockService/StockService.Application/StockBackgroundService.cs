using System.ComponentModel;
using Common.Exceptions;
using Common.Infrastructure;
using Common.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using StockService.Domain.Entities;
using StockService.Domain.Events;
using StockService.Domain.Interfaces;
using StockService.Infrastructure.Data;

namespace StockService.Application;

public class StockBackgroundService : BackgroundService
{
    private readonly ILogger<StockBackgroundService> _logger;
    private readonly EventBus _eventBus;

    
    private readonly IServiceScopeFactory _scopeFactory;

    public StockBackgroundService(ILogger<StockBackgroundService> logger, EventBus eventBus,
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
        await _eventBus.SubscribeAsync<OrderCreatedEvent>(
            queueName: RabbitMqConstants.OrderQueue,
            routingKey: RabbitMqConstants.OrderCreatedRoutingKey,
            onMessage: async (orderEvent) =>
            {
                _logger.LogInformation($"Received OrderCreatedEvent for Order Id: {orderEvent.OrderId}");
                // TODO: Process the event.

                //check if orderEvent items are exists
                foreach (var item in orderEvent.Items)
                {
                    
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<StockDbContext>>();

                        //check if item exists in stock
                        var repository = unitOfWork.GetRepository<IStockRepository>();
                        var stock = await repository.GetByIdAsync(item.ProductId);
                        if(stock == null) continue;

                        if (stock.Quantity < item.Quantity)
                        {
                            continue;   
                        }
                        //update the stock
                        stock.Quantity -= item.Quantity;
                        stock.UpdatedAt = DateTime.UtcNow;
                        repository.Update(stock);
                        // Use unitOfWork here
                        await unitOfWork.SaveChangesAsync();
                    
                        var retryPolicy = Policy
                            .Handle<MessaagingException>()
                            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

                        await retryPolicy.ExecuteAsync(async () =>
                        {
                            await _eventBus.PublishAsync(new StockUpdatedEvent()
                            {
                                ProductId = item.ProductId,
                                Quantity = item.Quantity,
                                UnitPrice = item.UnitPrice,
                                CreatedAt = DateTime.UtcNow,
                            }, RabbitMqConstants.StockUpdatedRoutingKey, RabbitMqConstants.StockQueueUpdated);
                        });
                        
                    }
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