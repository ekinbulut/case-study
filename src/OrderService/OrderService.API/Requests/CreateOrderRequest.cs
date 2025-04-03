namespace OrderService.API.Requests;

public class CreateOrderRequest
{
    public string CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
}