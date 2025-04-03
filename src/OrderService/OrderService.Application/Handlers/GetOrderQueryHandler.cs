using MediatR;
using OrderService.Application.DTOs;
using OrderService.Application.Queries;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Handlers;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderResult>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderResult> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        // Retrieve the order from the repository.
        var order = await _orderRepository.GetByIdAsync(request.OrderId);
        if (order == null)
        {
            return null;
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