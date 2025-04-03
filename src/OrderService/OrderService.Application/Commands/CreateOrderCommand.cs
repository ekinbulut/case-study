using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;

namespace OrderService.Application.Commands;

public class CreateOrderCommand : IRequest<OrderResult>
{
    public string CustomerId { get; set; }
    public List<OrderItem> Items { get; set; } 
}