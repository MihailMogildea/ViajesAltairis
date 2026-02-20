using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Reservations.Commands.CancelReservation;

public class CancelReservationHandler : IRequestHandler<CancelReservationCommand, Unit>
{
    private readonly IReservationApiClient _reservationApi;

    public CancelReservationHandler(IReservationApiClient reservationApi)
    {
        _reservationApi = reservationApi;
    }

    public async Task<Unit> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        await _reservationApi.CancelAsync(request.ReservationId, request.UserId, request.Reason, cancellationToken);
        return Unit.Value;
    }
}
