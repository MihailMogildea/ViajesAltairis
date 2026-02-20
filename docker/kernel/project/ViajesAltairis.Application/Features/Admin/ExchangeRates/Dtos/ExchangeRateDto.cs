namespace ViajesAltairis.Application.Features.Admin.ExchangeRates.Dtos;

public record ExchangeRateDto(long Id, long CurrencyId, decimal RateToEur, DateTime ValidFrom, DateTime ValidTo, DateTime CreatedAt);
