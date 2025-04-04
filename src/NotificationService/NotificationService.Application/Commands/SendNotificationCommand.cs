using MediatR;
using NotificationService.Application.DTOs;

namespace NotificationService.Application.Commands;

public class SendNotificationCommand : IRequest<NotificationResult>
{
    public Guid Id { get; set; } // notification id
}