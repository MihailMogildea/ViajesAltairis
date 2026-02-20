using MediatR;

namespace ViajesAltairis.Application.Features.Client.Reservations.Commands.RemoveReservationLine;

public class RemoveReservationLineCommand : IRequest<Unit>
{
    public long ReservationId { get; set; }
    public long LineId { get; set; }
}
