using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Commands;

public class CreateOrderCommand : IRequest<OrderResult>
{
    public string CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
}