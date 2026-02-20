using MediatR;

namespace ViajesAltairis.Application.Features.Client.Reservations.Commands.AddReservationLine;

public class AddReservationLineCommand : IRequest<long>
{
    public long ReservationId { get; set; }
    public long RoomConfigurationId { get; set; }
    public long BoardTypeId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int GuestCount { get; set; }
}
