using MediatR;
using Microsoft.AspNetCore.Mvc;
using StockService.API.Requests;
using StockService.Application.Commands;

namespace StockService.API.Controllers;

[ApiController]
[Route("api/v1/stock")]
public class StockController : ControllerBase
{
    private readonly IMediator _mediator;

    public StockController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST: api/stock
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateStockRequest request)
    {
        if (request == null)
        {
            return BadRequest();
        }
        
        var command = new CreateStockCommand()
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            Price = request.Price
        };
        
        await _mediator.Send(command);
        return Created();
    }
    
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] UpdateStockRequest request)
    {
        if (request == null)
        {
            return BadRequest();
        }
        
        var command = new UpdateStockCommand()
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            Price = request.Price
        };
        
        var result = await _mediator.Send(command);
        return result ? Ok() : NotFound();
    }
    
    [HttpPut("bulk")]
    public async Task<IActionResult> Put([FromBody] UpdateStockBulkRequest request)
    {
        if (request == null)
        {
            return BadRequest();
        }
        
        var command = new UpdateStockBulkCommand()
        {
            Prodcuts = request.Products
        };
        
        var result = await _mediator.Send(command);
        return result ? Ok() : NotFound();
    }

    // GET: api/order/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        // var query = new GetOrderQuery { OrderId = id };
        // var result = await _mediator.Send(query);
        // if (result == null)
        // {
        //     return NotFound();
        // }
        // return Ok(result);
        return Ok();
    }
}