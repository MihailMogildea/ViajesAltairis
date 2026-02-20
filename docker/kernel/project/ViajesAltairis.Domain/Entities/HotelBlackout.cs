namespace ViajesAltairis.Domain.Entities;

public class HotelBlackout : BaseEntity
{
    public long HotelId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? Reason { get; set; }

    public Hotel Hotel { get; set; } = null!;
}
