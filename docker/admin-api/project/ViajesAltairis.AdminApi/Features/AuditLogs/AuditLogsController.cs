using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.AuditLogs.Queries;

namespace ViajesAltairis.AdminApi.Features.AuditLogs;

[ApiController]
[Route("api/[controller]")]
public class AuditLogsController : ControllerBase
{
    private readonly ISender _sender;
    public AuditLogsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetAuditLogsQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetAuditLogByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }
}
