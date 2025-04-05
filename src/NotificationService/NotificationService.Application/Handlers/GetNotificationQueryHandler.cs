using Common.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.Application.DTOs;
using NotificationService.Application.Queries;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Data;

namespace NotificationService.Application.Handlers;

public class GetNotificationQueryHandler : IRequestHandler<GetNotificationQuery, NotificationResult>
{
    private readonly IUnitOfWork<NotificationDbContext> _unitOfWork;
    private readonly ILogger<GetNotificationQueryHandler> _logger;

    public GetNotificationQueryHandler(IUnitOfWork<NotificationDbContext> unitOfWork, ILogger<GetNotificationQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<NotificationResult> Handle(GetNotificationQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"GetNotificationQueryHandler: {request.Id}");
        var notificationRepository = _unitOfWork.GetRepository<INotificationRepository>();
        var notification = await notificationRepository.GetByIdAsync(request.Id);
        if (notification == null)
        {
            _logger.LogWarning($"GetNotificationQueryHandler: No notification with id: {request.Id}");
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