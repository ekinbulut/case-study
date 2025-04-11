namespace StockService.Application.DTOs;

public class StockResult
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public object Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Name { get; set; }
}