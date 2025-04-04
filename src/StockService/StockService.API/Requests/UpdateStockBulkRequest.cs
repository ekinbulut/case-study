using MediatR;
using StockService.Application.DTOs;

namespace StockService.API.Requests;

public class UpdateStockBulkRequest
{
    public List<StockItemDto> Products { get; set; }
}