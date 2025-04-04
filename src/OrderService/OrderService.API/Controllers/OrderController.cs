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
    public async Task<IActionResult> Post([FromBody] CreateOrderRequest request)
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
        return Ok(result);
    }
    
    // POST: api/order/complete
    [HttpPost("complete")]
    public async Task<IActionResult> Put([FromBody] CompleteOrderRequest request)
    {
        if (request == null)
        {
            return BadRequest();
        }

        var command = new CompleteOrderCommand()
        {
            OrderId = request.OrderId
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // GET: api/order/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
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