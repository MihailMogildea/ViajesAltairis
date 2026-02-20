namespace ViajesAltairis.Application.Features.Admin.Taxes.Dtos;

public record UpdateTaxRequest(long TaxTypeId, long? CountryId, long? AdministrativeDivisionId, long? CityId, decimal Rate, bool IsPercentage);
