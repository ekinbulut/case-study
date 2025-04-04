using MediatR;
using StockService.Application.DTOs;

namespace StockService.Application.Queries;

public class GetStockQuery : IRequest<StockResult>
{
    public Guid ProductId { get; set; }
}