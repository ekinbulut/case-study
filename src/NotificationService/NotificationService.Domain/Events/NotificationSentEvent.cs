namespace NotificationService.Domain.Events;

public class NotificationSentEvent
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Message { get; set; }
    public string NotificationType { get; set; }
    public string Status { get; set; }
}