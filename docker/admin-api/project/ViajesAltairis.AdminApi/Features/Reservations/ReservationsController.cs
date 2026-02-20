using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.Reservations.Commands;
using ViajesAltairis.Application.Features.Admin.Reservations.Dtos;
using ViajesAltairis.Application.Features.Admin.Reservations.Queries;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.AdminApi.Features.Reservations;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IReservationApiClient _reservationApiClient;
    private readonly ICurrentUserService _currentUser;

    public ReservationsController(ISender sender, IReservationApiClient reservationApiClient, ICurrentUserService currentUser)
    {
        _sender = sender;
        _reservationApiClient = reservationApiClient;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetReservationsQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetReservationByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{id:long}/lines")]
    public async Task<IActionResult> GetLines(long id) => Ok(await _sender.Send(new GetReservationLinesQuery(id)));

    [HttpGet("{id:long}/guests")]
    public async Task<IActionResult> GetGuests(long id) => Ok(await _sender.Send(new GetReservationGuestsQuery(id)));

    [HttpPatch("{id:long}/status")]
    public async Task<IActionResult> SetStatus(long id, SetStatusRequest request)
    {
        await _sender.Send(new SetReservationStatusCommand(id, request.StatusId));
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> CreateDraft(CreateReservationRequest request, CancellationToken cancellationToken)
    {
        var id = await _reservationApiClient.CreateDraftAsync(
            _currentUser.UserId!.Value, request.CurrencyCode, request.PromoCode,
            request.OwnerUserId,
            request.OwnerFirstName, request.OwnerLastName,
            request.OwnerEmail, request.OwnerPhone,
            request.OwnerTaxId, request.OwnerAddress,
            request.OwnerCity, request.OwnerPostalCode,
            request.OwnerCountry,
            cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPost("{id:long}/lines")]
    public async Task<IActionResult> AddLine(long id, AddLineRequest request, CancellationToken cancellationToken)
    {
        var lineId = await _reservationApiClient.AddLineAsync(
            id, request.RoomConfigurationId, request.BoardTypeId,
            request.CheckIn, request.CheckOut, request.GuestCount, cancellationToken);
        return Created($"api/reservations/{id}/lines/{lineId}", lineId);
    }

    [HttpDelete("{id:long}/lines/{lineId:long}")]
    public async Task<IActionResult> RemoveLine(long id, long lineId, CancellationToken cancellationToken)
    {
        await _reservationApiClient.RemoveLineAsync(id, lineId, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:long}/lines/{lineId:long}/guests")]
    public async Task<IActionResult> AddGuest(long id, long lineId, AddGuestRequest request, CancellationToken cancellationToken)
    {
        await _reservationApiClient.AddGuestAsync(id, lineId, request.FirstName, request.LastName, request.Email, request.Phone, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:long}/submit")]
    public async Task<IActionResult> Submit(long id, SubmitReservationRequest request, CancellationToken cancellationToken)
    {
        var result = await _reservationApiClient.SubmitAsync(
            id, request.PaymentMethodId, request.CardNumber,
            request.CardExpiry, request.CardCvv, request.CardHolderName, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:long}/cancel")]
    public async Task<IActionResult> Cancel(long id, CancelReservationRequest? request = null, CancellationToken cancellationToken = default)
    {
        await _reservationApiClient.CancelAsync(id, _currentUser.UserId!.Value, request?.Reason, cancellationToken);
        return NoContent();
    }
}
