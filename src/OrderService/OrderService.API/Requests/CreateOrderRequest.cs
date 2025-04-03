using OrderService.Application.DTOs;
using OrderService.Domain.Entities;

namespace OrderService.API.Requests;

public class CreateOrderRequest
{
    public string CustomerId { get; set; }
    public List<OrderItemDto> OrderItems { get; set; }
}