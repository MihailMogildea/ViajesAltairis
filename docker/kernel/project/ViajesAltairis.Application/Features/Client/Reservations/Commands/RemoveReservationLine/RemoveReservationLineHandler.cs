using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Reservations.Commands.RemoveReservationLine;

public class RemoveReservationLineHandler : IRequestHandler<RemoveReservationLineCommand, Unit>
{
    private readonly IReservationApiClient _reservationApi;

    public RemoveReservationLineHandler(IReservationApiClient reservationApi)
    {
        _reservationApi = reservationApi;
    }

    public async Task<Unit> Handle(RemoveReservationLineCommand request, CancellationToken cancellationToken)
    {
        await _reservationApi.RemoveLineAsync(request.ReservationId, request.LineId, cancellationToken);
        return Unit.Value;
    }
}
