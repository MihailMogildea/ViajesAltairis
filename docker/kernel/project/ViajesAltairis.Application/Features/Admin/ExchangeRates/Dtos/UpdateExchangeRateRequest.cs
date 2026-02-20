namespace ViajesAltairis.Application.Features.Admin.ExchangeRates.Dtos;

public record UpdateExchangeRateRequest(long CurrencyId, decimal RateToEur, DateTime ValidFrom, DateTime ValidTo);
