using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.ReservationStatuses.Queries;

namespace ViajesAltairis.AdminApi.Features.ReservationStatuses;

[ApiController]
[Route("api/[controller]")]
public class ReservationStatusesController : ControllerBase
{
    private readonly ISender _sender;
    public ReservationStatusesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetReservationStatusesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetReservationStatusByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }
}
