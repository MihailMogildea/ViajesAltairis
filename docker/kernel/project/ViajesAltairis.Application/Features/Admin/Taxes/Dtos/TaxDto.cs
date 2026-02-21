namespace ViajesAltairis.Application.Features.Admin.Taxes.Dtos;

public class TaxDto
{
    public long Id { get; init; }
    public long TaxTypeId { get; init; }
    public long? CountryId { get; init; }
    public long? AdministrativeDivisionId { get; init; }
    public long? CityId { get; init; }
    public decimal Rate { get; init; }
    public bool IsPercentage { get; init; }
    public bool Enabled { get; init; }
    public DateTime CreatedAt { get; init; }
}
