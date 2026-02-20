namespace ViajesAltairis.Application.Features.Admin.Countries.Dtos;

public record CreateCountryRequest(string IsoCode, string Name, long CurrencyId);
