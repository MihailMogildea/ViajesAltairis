using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Application.Reservations.Commands;
using ViajesAltairis.Application.Reservations.Queries;

namespace ViajesAltairis.ReservationsApi.Controllers;

[ApiController]
[Route("api/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReservationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("draft")]
    public async Task<ActionResult<long>> Create(CreateDraftReservationCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ReservationDetailResult>> GetById(long id)
    {
        var result = await _mediator.Send(new GetReservationByIdQuery(id));
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpPost("{id:long}/lines")]
    public async Task<ActionResult<long>> AddLine(long id, AddLineRequest request)
    {
        var command = new AddReservationLineCommand(
            id, request.RoomConfigurationId, request.BoardTypeId,
            request.CheckIn, request.CheckOut, request.GuestCount);
        var lineId = await _mediator.Send(command);
        return Created($"api/reservations/{id}/lines/{lineId}", lineId);
    }

    [HttpDelete("{id:long}/lines/{lineId:long}")]
    public async Task<IActionResult> RemoveLine(long id, long lineId)
    {
        await _mediator.Send(new RemoveReservationLineCommand(id, lineId));
        return NoContent();
    }

    [HttpPost("{id:long}/lines/{lineId:long}/guests")]
    public async Task<IActionResult> AddGuest(long id, long lineId, AddGuestRequest request)
    {
        await _mediator.Send(new AddReservationGuestCommand(
            id, lineId, request.FirstName, request.LastName, request.Email, request.Phone));
        return NoContent();
    }

    [HttpPost("{id:long}/submit")]
    public async Task<ActionResult<SubmitResult>> Submit(long id, SubmitRequest request)
    {
        var command = new SubmitReservationCommand(
            id, request.PaymentMethodId, request.CardNumber,
            request.CardExpiry, request.CardCvv, request.CardHolderName);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id:long}/cancel")]
    public async Task<IActionResult> Cancel(long id, CancelRequest? request = null)
    {
        await _mediator.Send(new CancelReservationCommand(id, request?.CancelledByUserId, request?.Reason));
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<ReservationListResult>> GetByUser(
        [FromQuery] long userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? status = null)
    {
        var result = await _mediator.Send(new GetReservationsByUserQuery(userId, page, pageSize, status));
        return Ok(result);
    }

    [HttpGet("lines/{lineId:long}/info")]
    public async Task<ActionResult<ReservationLineInfoResult>> GetLineInfo(long lineId)
    {
        var result = await _mediator.Send(new GetReservationLineInfoQuery(lineId));
        if (result is null)
            return NotFound();
        return Ok(result);
    }
}

// Thin request records for merging route params with body
public record AddLineRequest(long RoomConfigurationId, long BoardTypeId, DateTime CheckIn, DateTime CheckOut, int GuestCount);
public record AddGuestRequest(string FirstName, string LastName, string? Email, string? Phone);
public record SubmitRequest(long PaymentMethodId, string? CardNumber, string? CardExpiry, string? CardCvv, string? CardHolderName);
public record CancelRequest(long? CancelledByUserId, string? Reason);
