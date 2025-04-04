using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotificationService.API.Requests;
using NotificationService.Application.Commands;

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
            UserId = request.UserId,
            Message = request.Message,
            NotificationType = request.NotificationType
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

}