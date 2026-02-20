namespace ViajesAltairis.Application.Features.Admin.Taxes.Dtos;

public record TaxDto(long Id, long TaxTypeId, long? CountryId, long? AdministrativeDivisionId, long? CityId, decimal Rate, bool IsPercentage, bool Enabled, DateTime CreatedAt);
