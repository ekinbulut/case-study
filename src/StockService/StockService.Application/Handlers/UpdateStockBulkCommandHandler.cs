using Common.Exceptions;
using Common.Infrastructure;
using Common.Messaging;
using MediatR;
using Polly;
using StockService.Application.Commands;
using StockService.Domain.Events;
using StockService.Domain.Interfaces;
using StockService.Infrastructure.Data;

namespace StockService.Application.Handlers;

public class UpdateStockBulkCommandHandler : IRequestHandler<UpdateStockBulkCommand, bool>
{
    private readonly IUnitOfWork<StockDbContext> _unitOfWork;
    private readonly EventBus _eventBus;

    public UpdateStockBulkCommandHandler(EventBus eventBus, IUnitOfWork<StockDbContext> unitOfWork)
    {
        _eventBus = eventBus;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateStockBulkCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.GetRepository<IStockRepository>();
        foreach (var product in request.Prodcuts)
        {
            //check if product exists in stock
            var stock = await repository.GetByIdAsync(product.ProductId);
            if (stock == null) continue;
            var stockUpdateEvent = new StockUpdateEvent()
            {
                ProductId = product.ProductId,
                Quantity = product.Quantity,
            };
            // Publish StockUpdateEvent.
            var retryPolicy = Policy
                .Handle<MessaagingException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            
            await retryPolicy.ExecuteAsync(async () =>
            {
                await _eventBus.PublishAsync(stockUpdateEvent, RabbitMqConstants.StockUpdateRoutingKey, RabbitMqConstants.StockQueue);
            });
            
        }

        return true;
    }
}