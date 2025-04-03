namespace OrderService.Domain.Entities;

public enum OrderStatus
{
    Pending,
    Confirmed,
    Shipped,
    Delivered,
    Cancelled
}

public class Order
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string CustomerId { get; set; }

    public decimal TotalAmount
    {
        get
        {
            return Items.Select(x=> x.Quantity * x.UnitPrice).Sum();
        }
    }

    public OrderStatus Status { get; set; }
    
    public string ShippingAddress { get; set; }
    public string BillingAddress { get; set; }
    
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    
}