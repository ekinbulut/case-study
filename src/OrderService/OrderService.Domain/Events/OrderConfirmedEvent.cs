using OrderService.Domain.Entities;

namespace OrderService.Domain.Events;

public class OrderConfirmedEvent
{
    public Guid OrderId { get; set; }
    public string CustomerId { get; set; }
    public List<OrderItem> Items { get; set; }
}