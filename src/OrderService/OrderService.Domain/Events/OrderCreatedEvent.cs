using OrderService.Domain.Entities;

namespace OrderService.Domain.Events;

public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItem> Items { get; set; }
}