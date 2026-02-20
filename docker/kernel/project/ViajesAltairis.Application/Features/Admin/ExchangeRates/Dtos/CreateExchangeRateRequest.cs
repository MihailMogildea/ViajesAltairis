namespace ViajesAltairis.Application.Features.Admin.ExchangeRates.Dtos;

public record CreateExchangeRateRequest(long CurrencyId, decimal RateToEur, DateTime ValidFrom, DateTime ValidTo);
