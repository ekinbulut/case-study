using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Commands;

public class CompleteOrderCommand : IRequest<OrderResult>
{
    public Guid OrderId { get; set; }
}