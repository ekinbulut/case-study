namespace NotificationService.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public NotificationType NotificationType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; } = null;
    public string? ErrorMessage { get; set; } = null;
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
}

public enum NotificationType
{
    OrderCreated,
    OrderCompleted,
    OrderCancelled,
    OrderShipped,
    OrderDelivered,
    OrderReturned,
    OrderRefunded,
    OrderFailed
}

public enum NotificationStatus
{
    Pending,
    Sent,
    Failed
}