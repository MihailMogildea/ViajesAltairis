namespace ViajesAltairis.Application.Features.Admin.Countries.Dtos;

public record CountryDto(long Id, string IsoCode, string Name, long CurrencyId, bool Enabled, DateTime CreatedAt);
