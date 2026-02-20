namespace ViajesAltairis.Domain.Entities;

public class ReservationStatus : BaseEntity
{
    public string Name { get; set; } = null!;

    public ICollection<Reservation> Reservations { get; set; } = [];
}
