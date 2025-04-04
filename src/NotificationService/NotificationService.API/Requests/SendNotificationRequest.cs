namespace NotificationService.API.Requests;

public class SendNotificationRequest
{
    public Guid UserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string NotificationType { get; set; } = "OrderCreated";
}