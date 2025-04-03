using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.API.Requests;
using OrderService.Application.Commands;
using OrderService.Application.Queries;
using OrderService.Domain.Entities;

namespace OrderService.API.Controllers;

[ApiController]
[Route("api/v1/order")]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST: api/order
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        if (request == null)
        {
            return BadRequest();
        }

        var command = new CreateOrderCommand()
        {
            CustomerId = request.CustomerId,
            Items = request.OrderItems.Select(x=> new OrderItem(){
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice
            }).ToList()
        };
        

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
    }

    // GET: api/order/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var query = new GetOrderQuery { OrderId = id };
        var result = await _mediator.Send(query);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}