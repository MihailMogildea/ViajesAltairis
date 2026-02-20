namespace ViajesAltairis.Domain.Entities;

public class HotelAmenity : BaseEntity
{
    public long HotelId { get; set; }
    public long AmenityId { get; set; }

    public Hotel Hotel { get; set; } = null!;
    public Amenity Amenity { get; set; } = null!;
}
