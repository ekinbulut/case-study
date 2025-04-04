using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotificationService.API.Requests;
using NotificationService.Application.Commands;
using NotificationService.Application.Queries;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/v1/notification")]
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST: api/notification
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SendNotificationRequest request)
    {
        if (request == null)
        {
            return BadRequest();
        }

        var command = new SendNotificationCommand()
        {
            Id = request.Id
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    // GET: api/notification/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetNotificationQuery { Id = id };
        var result = await _mediator.Send(query);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

}