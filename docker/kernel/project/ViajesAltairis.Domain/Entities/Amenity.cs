namespace ViajesAltairis.Domain.Entities;

public class Amenity : BaseEntity
{
    public long CategoryId { get; set; }
    public string Name { get; set; } = null!;

    public AmenityCategory Category { get; set; } = null!;
    public ICollection<HotelAmenity> HotelAmenities { get; set; } = [];
    public ICollection<HotelProviderRoomTypeAmenity> HotelProviderRoomTypeAmenities { get; set; } = [];
}
