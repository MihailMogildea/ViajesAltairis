namespace ViajesAltairis.Domain.Entities;

public class HotelProvider : BaseEntity
{
    public long HotelId { get; set; }
    public long ProviderId { get; set; }
    public bool Enabled { get; set; }

    public Hotel Hotel { get; set; } = null!;
    public Provider Provider { get; set; } = null!;
    public ICollection<HotelProviderRoomType> HotelProviderRoomTypes { get; set; } = [];
}
