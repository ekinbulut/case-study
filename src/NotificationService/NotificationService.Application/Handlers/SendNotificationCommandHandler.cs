using Common.Exceptions;
using Common.Infrastructure;
using Common.Messaging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Commands;
using NotificationService.Application.DTOs;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Events;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Data;
using Polly;

namespace NotificationService.Application.Handlers;

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, NotificationResult>
{
    private readonly IUnitOfWork<NotificationDbContext> _unitOfWork;
    private readonly EventBus _eventBus;

    public SendNotificationCommandHandler(EventBus eventBus, IUnitOfWork<NotificationDbContext> unitOfWork)
    {
        _eventBus = eventBus;
        _unitOfWork = unitOfWork;
    }

    public async Task<NotificationResult> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        Notification? notification = null;
        // Persist the new notification.
        try
        {
            var notificationRepository = _unitOfWork.GetRepository<INotificationRepository>();
            notification = await notificationRepository.GetByIdAsync(request.Id);
            if (notification == null)
            {
                throw new NotFoundException($"Notification with ID {request.Id} not found.");
            }
            
            // Publish NotificationCreatedEvent after successful save.
            var notificationCreatedEvent = new NotificationCreatedEvent
            {
                Id = notification.Id,
                UserId = notification.CustomerId,
                Message = notification.Message,
                NotificationType = notification.NotificationType.ToString(),
                Status = notification.Status.ToString()
            };
        
            var retryPolicy = Policy
                .Handle<MessaagingException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            await retryPolicy.ExecuteAsync(async () =>

                _eventBus.PublishAsync(notificationCreatedEvent, RabbitMqConstants.NotificationRoutingKey,
                    RabbitMqConstants.NotificationQueue));
            
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while saving the notification. Please try again later.", ex);
        }
        
        return new NotificationResult(){
            Id = notification.Id.ToString(),
            Status = notification.Status.ToString()
        };
    }
    
    public static NotificationType? ConvertStringToEnum(string type)
    {
        if (Enum.TryParse<NotificationType>(type, true, out var result))
        {
            return result;
        }
        return null; // Or handle the error as you wish
    }
}