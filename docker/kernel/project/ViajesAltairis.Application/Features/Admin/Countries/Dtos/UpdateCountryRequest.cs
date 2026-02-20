namespace ViajesAltairis.Application.Features.Admin.Countries.Dtos;

public record UpdateCountryRequest(string IsoCode, string Name, long CurrencyId);
