namespace ViajesAltairis.Application.Features.Admin.Currencies.Dtos;

public record UpdateCurrencyRequest(string IsoCode, string Name, string Symbol);
