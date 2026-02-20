namespace ViajesAltairis.Application.Features.Admin.Currencies.Dtos;

public record CurrencyDto(long Id, string IsoCode, string Name, string Symbol, DateTime CreatedAt);
