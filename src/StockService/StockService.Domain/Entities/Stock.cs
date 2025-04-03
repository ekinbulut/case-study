namespace StockService.Domain.Entities;

public class Stock
{
    public Guid Id { get; set; }
    public List<StockItem> Items { get; set; } = new List<StockItem>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}