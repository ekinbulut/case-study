using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Queries;

public class GetOrderQuery : IRequest<OrderResult>
{
    public Guid OrderId { get; set; }
}