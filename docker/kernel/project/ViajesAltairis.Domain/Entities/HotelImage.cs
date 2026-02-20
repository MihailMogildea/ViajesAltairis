namespace ViajesAltairis.Domain.Entities;

public class HotelImage : BaseEntity
{
    public long HotelId { get; set; }
    public string Url { get; set; } = null!;
    public string? AltText { get; set; }
    public int SortOrder { get; set; }

    public Hotel Hotel { get; set; } = null!;
}
