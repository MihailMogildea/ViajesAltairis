namespace ViajesAltairis.Application.Interfaces;

public interface ICurrencyConverter
{
    Task<(decimal convertedAmount, long exchangeRateId)> ConvertAsync(
        decimal amount, long sourceCurrencyId, long targetCurrencyId, CancellationToken ct = default);
}
