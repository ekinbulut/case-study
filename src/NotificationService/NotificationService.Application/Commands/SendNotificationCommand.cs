using MediatR;
using NotificationService.Application.DTOs;

namespace NotificationService.Application.Commands;

public class SendNotificationCommand : IRequest<NotificationResult>
{
    public Guid UserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string NotificationType { get; set; } = "OrderCreated"; // Could be "OrderCompleted", etc.
}