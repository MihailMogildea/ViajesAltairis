namespace ViajesAltairis.Domain.Entities;

public class Review : AuditableEntity
{
    public long ReservationId { get; set; }
    public long UserId { get; set; }
    public long HotelId { get; set; }
    public byte Rating { get; set; }
    public string? Title { get; set; }
    public string? Comment { get; set; }
    public bool Visible { get; set; }

    public Reservation Reservation { get; set; } = null!;
    public User User { get; set; } = null!;
    public Hotel Hotel { get; set; } = null!;
    public ReviewResponse? ReviewResponse { get; set; }
}
