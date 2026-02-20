using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Reservations.Commands.AddReservationLine;

public class AddReservationLineHandler : IRequestHandler<AddReservationLineCommand, long>
{
    private readonly IReservationApiClient _reservationApi;

    public AddReservationLineHandler(IReservationApiClient reservationApi)
    {
        _reservationApi = reservationApi;
    }

    public async Task<long> Handle(AddReservationLineCommand request, CancellationToken cancellationToken)
    {
        return await _reservationApi.AddLineAsync(
            request.ReservationId,
            request.RoomConfigurationId,
            request.BoardTypeId,
            request.CheckIn,
            request.CheckOut,
            request.GuestCount,
            cancellationToken);
    }
}
