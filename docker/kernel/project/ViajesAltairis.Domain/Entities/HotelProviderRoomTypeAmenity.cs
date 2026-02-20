namespace ViajesAltairis.Domain.Entities;

public class HotelProviderRoomTypeAmenity : BaseEntity
{
    public long HotelProviderRoomTypeId { get; set; }
    public long AmenityId { get; set; }

    public HotelProviderRoomType HotelProviderRoomType { get; set; } = null!;
    public Amenity Amenity { get; set; } = null!;
}
