using Common.Infrastructure;
using MediatR;
using StockService.Application.DTOs;
using StockService.Application.Queries;
using StockService.Domain.Interfaces;
using StockService.Infrastructure.Data;

namespace StockService.Application.Handlers;

public class GetStockQueryHandler : IRequestHandler<GetStockQuery, StockResult>
{
    private IUnitOfWork<StockDbContext> _unitOfWork;

    public GetStockQueryHandler(IUnitOfWork<StockDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<StockResult> Handle(GetStockQuery request, CancellationToken cancellationToken)
    {
        var stockRepository = _unitOfWork.GetRepository<IStockRepository>();
        var stock = await stockRepository.GetByIdAsync(request.ProductId);
        if (stock == null)
        {
            return null;
        }
        return new StockResult
        {
            ProductId = stock.ProductId.ToString(),
            Quantity = stock.Quantity,
            Price = stock.UnitPrice,
            CreatedAt = stock.CreatedAt,
            UpdatedAt = stock.UpdatedAt
        };
    }
}