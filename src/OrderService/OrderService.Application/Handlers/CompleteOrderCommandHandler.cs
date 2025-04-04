using Common.Exceptions;
using Common.Infrastructure;
using Common.Messaging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Events;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Data;
using Polly;

namespace OrderService.Application.Handlers;

public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommand, OrderResult?>
{
    private readonly IUnitOfWork<OrderDbContext> _unitOfWork;
    private readonly EventBus _eventBus;

    public CompleteOrderCommandHandler(IUnitOfWork<OrderDbContext> unitOfWork, EventBus eventBus)
    {
        _unitOfWork = unitOfWork;
        _eventBus = eventBus;
    }

    public async Task<OrderResult?> Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
    {
        Order? order = null;
        try
        {
            var respository = _unitOfWork.GetRepository<IOrderRepository>();
            order = await respository.GetByIdAsync(request.OrderId);
            if (order == null)
            {
                return null;
            }
            // Update the order status to Completed.
            order.Status = OrderStatus.Confirmed;
            respository.Update(order);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("An error occurred while saving the order. Please try again later.", ex);
        }
        
        // Map the Order entity to the OrderResult DTO.
        var result = new OrderResult
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            CustomerId = order.CustomerId,
            TotalAmount = order.Items.Select(x => x.Quantity * x.UnitPrice).Sum(),
            Status = order.Status.ToString()
        };
            
        // Publish OrderCompletedEvent after successful save.
        var orderCompletedEvent = new OrderConfirmedEvent()
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            Items = order.Items
        };
            
        var retryPolicy = Policy
            .Handle<MessaagingException>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            
        await retryPolicy.ExecuteAsync(async () =>
        {
            await _eventBus.PublishAsync(orderCompletedEvent, RabbitMqConstants.OrderConfirmedRoutingKey,
                RabbitMqConstants.NotificationQueue);
        });
        
        return result;
        
    }
}