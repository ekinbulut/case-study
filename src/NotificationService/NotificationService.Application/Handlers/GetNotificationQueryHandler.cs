using Common.Infrastructure;
using MediatR;
using NotificationService.Application.DTOs;
using NotificationService.Application.Queries;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Data;

namespace NotificationService.Application.Handlers;

public class GetNotificationQueryHandler : IRequestHandler<GetNotificationQuery, NotificationResult>
{
    private IUnitOfWork<NotificationDbContext> _unitOfWork;

    public GetNotificationQueryHandler(IUnitOfWork<NotificationDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<NotificationResult> Handle(GetNotificationQuery request, CancellationToken cancellationToken)
    {
        var notificationRepository = _unitOfWork.GetRepository<INotificationRepository>();
        var notification = await notificationRepository.GetByIdAsync(request.Id);
        if (notification == null)
        {
            return null;
        }
        return new NotificationResult
        {
            Id = notification.Id.ToString(),
            CustomerId = notification.CustomerId,
            Message = notification.Message,
            NotificationType = notification.NotificationType.ToString(),
            Status = notification.Status.ToString(),
            CreatedAt = notification.CreatedAt,
            UpdatedAt = notification.UpdatedAt
        };
    }
}