using MediatR;
using NotificationService.Application.DTOs;

namespace NotificationService.Application.Queries;

public class GetNotificationQuery : IRequest<NotificationResult>
{
    public Guid Id { get; set; }
}