using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.NotificationLogs.Queries;

namespace ViajesAltairis.AdminApi.Features.NotificationLogs;

[ApiController]
[Route("api/[controller]")]
public class NotificationLogsController : ControllerBase
{
    private readonly ISender _sender;
    public NotificationLogsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetNotificationLogsQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetNotificationLogByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }
}
