namespace ViajesAltairis.Domain.Entities;

public class Tax : BaseEntity
{
    public long TaxTypeId { get; set; }
    public long? CountryId { get; set; }
    public long? AdministrativeDivisionId { get; set; }
    public long? CityId { get; set; }
    public decimal Rate { get; set; }
    public bool IsPercentage { get; set; }
    public bool Enabled { get; set; }

    public TaxType TaxType { get; set; } = null!;
    public Country? Country { get; set; }
    public AdministrativeDivision? AdministrativeDivision { get; set; }
    public City? City { get; set; }
}
