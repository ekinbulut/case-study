namespace NotificationService.Domain.Events;

public class OrderConfirmedEvent
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
}