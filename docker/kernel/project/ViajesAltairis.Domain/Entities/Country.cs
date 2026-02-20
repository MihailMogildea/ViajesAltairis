namespace ViajesAltairis.Domain.Entities;

public class Country : BaseEntity
{
    public string IsoCode { get; set; } = null!;
    public string Name { get; set; } = null!;
    public long CurrencyId { get; set; }
    public bool Enabled { get; set; }

    public Currency Currency { get; set; } = null!;
    public ICollection<AdministrativeDivision> AdministrativeDivisions { get; set; } = [];
}
