namespace ViajesAltairis.Domain.Entities;

public class CancellationPolicy : BaseEntity
{
    public long HotelId { get; set; }
    public int FreeCancellationHours { get; set; }
    public decimal PenaltyPercentage { get; set; }
    public bool Enabled { get; set; }

    public Hotel Hotel { get; set; } = null!;
}
