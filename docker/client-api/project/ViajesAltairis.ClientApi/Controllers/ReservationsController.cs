using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Client.Reservations.Commands.AddReservationLine;
using ViajesAltairis.Application.Features.Client.Reservations.Commands.CancelReservation;
using ViajesAltairis.Application.Features.Client.Reservations.Commands.CreateDraftReservation;
using ViajesAltairis.Application.Features.Client.Reservations.Commands.AddReservationGuest;
using ViajesAltairis.Application.Features.Client.Reservations.Commands.RemoveReservationLine;
using ViajesAltairis.Application.Features.Client.Reservations.Commands.SubmitReservation;
using ViajesAltairis.Application.Features.Client.Reservations.Queries.GetMyReservations;
using ViajesAltairis.Application.Features.Client.Reservations.Queries.GetReservationDetail;

namespace ViajesAltairis.ClientApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Client")]
public class ReservationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReservationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyReservations([FromQuery] GetMyReservationsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(long id)
    {
        var result = await _mediator.Send(new GetReservationDetailQuery { ReservationId = id });
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDraft([FromBody] CreateDraftReservationCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetDetail), new { id = result }, result);
    }

    [HttpPost("{id}/lines")]
    public async Task<IActionResult> AddLine(long id, [FromBody] AddReservationLineCommand command)
    {
        command.ReservationId = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}/lines/{lineId}")]
    public async Task<IActionResult> RemoveLine(long id, long lineId)
    {
        await _mediator.Send(new RemoveReservationLineCommand { ReservationId = id, LineId = lineId });
        return NoContent();
    }

    [HttpPost("{id}/lines/{lineId}/guests")]
    public async Task<IActionResult> AddGuest(long id, long lineId, [FromBody] AddReservationGuestCommand command)
    {
        command.ReservationId = id;
        command.LineId = lineId;
        await _mediator.Send(command);
        return Ok();
    }

    [HttpPost("{id}/submit")]
    public async Task<IActionResult> Submit(long id, [FromBody] SubmitReservationCommand command)
    {
        command.ReservationId = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(long id, [FromBody] CancelReservationRequest? request)
    {
        var userId = long.Parse(User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("Missing user ID claim"));
        await _mediator.Send(new CancelReservationCommand { ReservationId = id, UserId = userId, Reason = request?.Reason });
        return NoContent();
    }
}

public class CancelReservationRequest
{
    public string? Reason { get; set; }
}
