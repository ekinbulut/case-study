using System.ComponentModel;
using Common.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StockService.Domain.Events;

namespace StockService.Application;

public class StockBackgroundService : BackgroundService
{
    private readonly ILogger<StockBackgroundService> _logger;
    private readonly EventBus _eventBus;

    public StockBackgroundService(ILogger<StockBackgroundService> logger, EventBus eventBus)
    {
        _logger = logger;
        _eventBus = eventBus;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Stock background service is starting.");

        // Subscribe to the OrderCreatedEvent using the EventBus.
        await _eventBus.SubscribeAsync<OrderCreatedEvent>(
            queueName: RabbitMqConstants.StockQueue,
            routingKey: RabbitMqConstants.OrderCreatedRoutingKey,
            onMessage: async (orderEvent) =>
            {
                _logger.LogInformation($"Received OrderCreatedEvent for Order Id: {orderEvent.OrderId}");
                // TODO: Process the event.
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