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

public class StockUpdateBackgroundService : BackgroundService
{
    private readonly ILogger<StockUpdateBackgroundService> _logger;
    private readonly EventBus _eventBus;


    private readonly IServiceScopeFactory _scopeFactory;

    public StockUpdateBackgroundService(ILogger<StockUpdateBackgroundService> logger, EventBus eventBus,
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
        await _eventBus.SubscribeAsync<StockUpdateEvent>(
            queueName: RabbitMqConstants.StockQueue,
            routingKey: RabbitMqConstants.StockUpdateRoutingKey,
            onMessage: async (stockUpdateEvent) =>
            {
                _logger.LogInformation($"Received StockUpdateEvent for Product Id: {stockUpdateEvent.ProductId}");
                // TODO: Process the event.
                using (var scope = _scopeFactory.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<StockDbContext>>();

                    var repository = unitOfWork.GetRepository<IStockRepository>();
                    var stock = await repository.GetByIdAsync(stockUpdateEvent.ProductId);

                    //update the stock
                    stock.Quantity -= stockUpdateEvent.Quantity;
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
                            ProductId = stockUpdateEvent.ProductId,
                            Quantity = stockUpdateEvent.Quantity,
                            UnitPrice = stock.UnitPrice,
                            CreatedAt = DateTime.UtcNow,
                        }, RabbitMqConstants.StockUpdatedRoutingKey, RabbitMqConstants.StockQueueUpdated);
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