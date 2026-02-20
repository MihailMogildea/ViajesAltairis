using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.ExternalClient.Reservations.Commands;
using ViajesAltairis.Application.Features.ExternalClient.Reservations.Dtos;
using ViajesAltairis.Application.Features.ExternalClient.Reservations.Queries;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.ExternalClientApi.Reservations;

[ApiController]
[Route("api/reservations")]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReservationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDraft([FromBody] CreatePartnerDraftRequest request)
    {
        var userId = GetRequiredClaim("userId");
        var command = new CreatePartnerDraftCommand(
            request.OwnerFirstName, request.OwnerLastName, request.OwnerEmail,
            request.OwnerPhone, request.OwnerTaxId,
            request.CurrencyCode, request.PromoCode)
        { BookedByUserId = userId };

        var reservationId = await _mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, new { ReservationId = reservationId });
    }

    [HttpPost("{id:long}/lines")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddLine(long id, [FromBody] AddPartnerLineRequest request)
    {
        var partnerId = GetRequiredClaim("businessPartnerId");
        var command = new AddPartnerLineCommand(
            id, request.RoomConfigurationId, request.BoardTypeId,
            request.CheckIn, request.CheckOut, request.GuestCount)
        { BusinessPartnerId = partnerId };

        var lineId = await _mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, new { LineId = lineId });
    }

    [HttpDelete("{id:long}/lines/{lineId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveLine(long id, long lineId)
    {
        var partnerId = GetRequiredClaim("businessPartnerId");
        var command = new RemovePartnerLineCommand(id, lineId) { BusinessPartnerId = partnerId };
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("{id:long}/lines/{lineId:long}/guests")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddGuest(long id, long lineId, [FromBody] AddPartnerGuestRequest request)
    {
        var partnerId = GetRequiredClaim("businessPartnerId");
        var command = new AddPartnerGuestCommand(
            id, lineId, request.FirstName, request.LastName, request.Email, request.Phone)
        { BusinessPartnerId = partnerId };

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("{id:long}/submit")]
    [ProducesResponseType(typeof(SubmitResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Submit(long id, [FromBody] SubmitPartnerReservationRequest request)
    {
        var partnerId = GetRequiredClaim("businessPartnerId");
        var command = new SubmitPartnerReservationCommand(
            id, request.PaymentMethodId,
            request.CardNumber, request.CardExpiry, request.CardCvv, request.CardHolderName)
        { BusinessPartnerId = partnerId };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id:long}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancel(long id, [FromBody] CancelReservationRequest? request)
    {
        var userId = GetRequiredClaim("userId");
        var partnerId = GetRequiredClaim("businessPartnerId");

        var command = new CancelPartnerReservationCommand(id, request?.Reason)
        {
            CancelledByUserId = userId,
            BusinessPartnerId = partnerId
        };

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetPartnerReservationsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] int? statusId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var partnerId = GetRequiredClaim("businessPartnerId");
        var result = await _mediator.Send(new GetPartnerReservationsQuery(partnerId, statusId, page, pageSize));
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ReservationDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetail(long id)
    {
        var partnerId = GetRequiredClaim("businessPartnerId");
        var result = await _mediator.Send(new GetPartnerReservationQuery(id, partnerId));
        return result is null ? NotFound() : Ok(result);
    }

    private long GetRequiredClaim(string claimType)
    {
        var value = User.FindFirst(claimType)?.Value;
        if (value is null || !long.TryParse(value, out var result))
            throw new UnauthorizedAccessException($"Missing required claim: {claimType}");
        return result;
    }
}

public record CancelReservationRequest(string? Reason);
