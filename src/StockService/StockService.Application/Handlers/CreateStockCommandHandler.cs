using Common.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StockService.Application.Commands;
using StockService.Domain.Interfaces;
using StockService.Infrastructure.Data;

namespace StockService.Application.Handlers;

public class CreateStockCommandHandler : IRequestHandler<CreateStockCommand>
{
    private readonly IUnitOfWork<StockDbContext> _unitOfWork;

    public CreateStockCommandHandler(IUnitOfWork<StockDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CreateStockCommand request, CancellationToken cancellationToken)
    {
        var stock = new Domain.Entities.Stock
        {
            Items = new List<Domain.Entities.StockItem>()
            {
                new Domain.Entities.StockItem
                {
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                }
            },
            CreatedAt = DateTime.Now,
        };
        
        try
        {
            // Persist the new order.
            var repository = _unitOfWork.GetRepository<IStockRepository>();
            await repository.AddAsync(stock);
        }
        catch (DbUpdateException ex)
        {
            // Handle the error (e.g., log it) and throw a new exception or return an error result.
            throw new Exception("An error occurred while saving the order. Please try again later.", ex);
        }
    }
}