using Common.Messaging;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Application.Queries;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Data;

namespace OrderService.Application.Handlers;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderResult>
{
    private readonly IUnitOfWork<OrderDbContext> _unitOfWork;
    private readonly EventBus _eventBus;

    public GetOrderQueryHandler(IUnitOfWork<OrderDbContext> unitOfWork, EventBus eventBus)
    {
        _unitOfWork = unitOfWork;
        _eventBus = eventBus;
    }

    public async Task<OrderResult> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        // Retrieve the order from the repository.
        var _orderRepository = _unitOfWork.GetRepository<IOrderRepository>();
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
            TotalAmount = order.Items.Select(x=> x.Quantity * x.UnitPrice).Sum(),
            Status = order.Status.ToString()
        };

        return result;
    }
}