namespace StockService.API.Requests;

public class UpdateStockRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public double? Price { get; set; }
}