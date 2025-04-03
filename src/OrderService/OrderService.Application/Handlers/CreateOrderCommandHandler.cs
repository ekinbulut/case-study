using Common.Exceptions;
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

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderResult>
{
    private readonly IUnitOfWork<OrderDbContext> _unitOfWork;
    private readonly EventBus _eventBus;

    public CreateOrderCommandHandler(IUnitOfWork<OrderDbContext> unitOfWork, EventBus eventBus)
    {
        _unitOfWork = unitOfWork;
        _eventBus = eventBus;
    }

    public async Task<OrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Create a new Order entity with initial values.
        var order = new Order
        {
            OrderDate = DateTime.UtcNow,
            CustomerId = request.CustomerId,
            Status = OrderStatus.Pending,
            Items = request.Items
        };

        try
        {
            // Persist the new order.
            var orderRepository = _unitOfWork.GetRepository<IOrderRepository>();
            await orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Handle the error (e.g., log it) and throw a new exception or return an error result.
            throw new Exception("An error occurred while saving the order. Please try again later.", ex);
        }

        try
        {
            // Publish OrderCreatedEvent after successful save.
            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                CustomerId = order.CustomerId,
                TotalAmount = order.TotalAmount,
                Items = order.Items
            };
            
            var retryPolicy = Policy
                .Handle<MessaagingException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            
            await retryPolicy.ExecuteAsync(async () =>
            {
                await _eventBus.PublishAsync(orderCreatedEvent, RabbitMqConstants.OrderCreatedRoutingKey);
            });
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while saving the order. Please try again later.", ex);
        }

        // Map the Order entity to the OrderResult DTO.
        var result = new OrderResult
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString()
        };

        return result;
    }
}