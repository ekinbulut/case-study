using MediatR;

namespace NotificationService.Application.Commands;

public class CreateStockCommand : IRequest, IRequest<Guid>
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}