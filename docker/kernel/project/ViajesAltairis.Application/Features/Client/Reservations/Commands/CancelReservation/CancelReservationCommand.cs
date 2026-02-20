using MediatR;

namespace ViajesAltairis.Application.Features.Client.Reservations.Commands.CancelReservation;

public class CancelReservationCommand : IRequest<Unit>
{
    public long ReservationId { get; set; }
    public long UserId { get; set; }
    public string? Reason { get; set; }
}
