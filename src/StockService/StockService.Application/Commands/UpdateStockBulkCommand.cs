using MediatR;
using StockService.Application.DTOs;

namespace StockService.Application.Commands;

public class UpdateStockBulkCommand : IRequest<bool>
{
    public List<StockItemDto> Prodcuts { get; set; }
}