using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.Cancellations.Queries;

namespace ViajesAltairis.AdminApi.Features.Cancellations;

[ApiController]
[Route("api/[controller]")]
public class CancellationsController : ControllerBase
{
    private readonly ISender _sender;
    public CancellationsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetCancellationsQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetCancellationByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }
}
