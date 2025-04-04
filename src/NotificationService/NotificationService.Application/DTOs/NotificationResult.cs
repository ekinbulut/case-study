namespace NotificationService.Application.DTOs;

public class NotificationResult
{
    public string Id { get; set; }
    public string Status { get; set; }
    public Guid CustomerId { get; set; }
    public string Message { get; set; }
    public string NotificationType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}