namespace ViajesAltairis.Domain.Entities;

public class City : BaseEntity
{
    public long AdministrativeDivisionId { get; set; }
    public string Name { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public bool Enabled { get; set; }

    public AdministrativeDivision AdministrativeDivision { get; set; } = null!;
    public ICollection<Hotel> Hotels { get; set; } = [];
}
