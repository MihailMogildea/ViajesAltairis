using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Reservations.Commands.AddReservationGuest;

public class AddReservationGuestHandler : IRequestHandler<AddReservationGuestCommand, Unit>
{
    private readonly IReservationApiClient _reservationApi;

    public AddReservationGuestHandler(IReservationApiClient reservationApi)
    {
        _reservationApi = reservationApi;
    }

    public async Task<Unit> Handle(AddReservationGuestCommand request, CancellationToken cancellationToken)
    {
        await _reservationApi.AddGuestAsync(
            request.ReservationId,
            request.LineId,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            cancellationToken);
        return Unit.Value;
    }
}
