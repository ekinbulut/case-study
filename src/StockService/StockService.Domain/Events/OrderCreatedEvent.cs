using StockService.Domain.Entities;

namespace StockService.Domain.Events;

public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public List<Stock> Items { get; set; }
}