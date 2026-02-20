namespace ViajesAltairis.Domain.Entities;

public class Cancellation : BaseEntity
{
    public long ReservationId { get; set; }
    public long CancelledByUserId { get; set; }
    public string? Reason { get; set; }
    public decimal PenaltyPercentage { get; set; }
    public decimal PenaltyAmount { get; set; }
    public decimal RefundAmount { get; set; }
    public long CurrencyId { get; set; }

    public Reservation Reservation { get; set; } = null!;
    public User CancelledByUser { get; set; } = null!;
    public Currency Currency { get; set; } = null!;
}
