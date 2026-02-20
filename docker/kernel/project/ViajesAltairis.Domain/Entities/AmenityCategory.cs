namespace ViajesAltairis.Domain.Entities;

public class AmenityCategory : BaseEntity
{
    public string Name { get; set; } = null!;

    public ICollection<Amenity> Amenities { get; set; } = [];
}
