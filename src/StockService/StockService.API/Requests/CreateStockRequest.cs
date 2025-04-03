using StockService.Application.DTOs;

namespace StockService.API.Requests;

public class CreateStockRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}