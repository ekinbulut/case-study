using MediatR;

namespace StockService.Application.Commands;

public class UpdateStockCommand : IRequest<bool>
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public double? Price { get; set; }
}