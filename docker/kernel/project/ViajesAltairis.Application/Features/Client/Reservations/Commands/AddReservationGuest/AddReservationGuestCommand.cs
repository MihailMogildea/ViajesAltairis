using MediatR;

namespace ViajesAltairis.Application.Features.Client.Reservations.Commands.AddReservationGuest;

public class AddReservationGuestCommand : IRequest<Unit>
{
    public long ReservationId { get; set; }
    public long LineId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
}
