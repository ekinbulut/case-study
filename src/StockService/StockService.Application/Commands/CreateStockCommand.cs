using MediatR;

namespace StockService.Application.Commands;

public class CreateStockCommand : IRequest, IRequest<Guid>
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string Name { get; set; }
}