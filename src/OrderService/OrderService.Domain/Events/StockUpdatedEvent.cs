namespace OrderService.Domain.Events;

public class StockUpdatedEvent
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid OrderId { get; set; }

}