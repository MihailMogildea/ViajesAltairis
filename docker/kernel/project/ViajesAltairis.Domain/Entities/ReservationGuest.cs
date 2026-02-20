namespace ViajesAltairis.Domain.Entities;

public class ReservationGuest : BaseEntity
{
    public long ReservationLineId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }

    public ReservationLine ReservationLine { get; set; } = null!;
}
