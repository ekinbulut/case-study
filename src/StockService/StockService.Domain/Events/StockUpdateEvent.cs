namespace StockService.Domain.Events;

public class StockUpdateEvent
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}