using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Data;

namespace OrderService.Application.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderResult>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork<OrderDbContext> _unitOfWork;

    public CreateOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork<OrderDbContext> unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Create a new Order entity with initial values.
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderDate = DateTime.UtcNow,
            CustomerId = request.CustomerId,
            TotalAmount = request.TotalAmount,
            Status = OrderStatus.Pending
        };

        try
        {
            // Persist the new order.
            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Handle the error (e.g., log it) and throw a new exception or return an error result.
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