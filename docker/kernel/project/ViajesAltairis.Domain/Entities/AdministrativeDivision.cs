namespace ViajesAltairis.Domain.Entities;

public class AdministrativeDivision : BaseEntity
{
    public long CountryId { get; set; }
    public long? ParentId { get; set; }
    public string Name { get; set; } = null!;
    public long TypeId { get; set; }
    public byte Level { get; set; }
    public bool Enabled { get; set; }

    public Country Country { get; set; } = null!;
    public AdministrativeDivision? Parent { get; set; }
    public AdministrativeDivisionType Type { get; set; } = null!;
    public ICollection<AdministrativeDivision> Children { get; set; } = [];
    public ICollection<City> Cities { get; set; } = [];
    public ICollection<SeasonalMargin> SeasonalMargins { get; set; } = [];
}
