using Dapper;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Infrastructure.Currency;

public class CurrencyConverter : ICurrencyConverter
{
    private const long EurCurrencyId = 1;
    private readonly IDbConnectionFactory _connectionFactory;

    public CurrencyConverter(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<(decimal convertedAmount, long exchangeRateId)> ConvertAsync(
        decimal amount, long sourceCurrencyId, long targetCurrencyId, CancellationToken ct = default)
    {
        if (sourceCurrencyId == targetCurrencyId)
            return (amount, 0);

        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT id, currency_id, rate_to_eur
            FROM exchange_rate
            WHERE currency_id IN @CurrencyIds
              AND valid_from <= NOW() AND valid_to > NOW()
            ORDER BY valid_from DESC
            """;

        var currencyIds = new List<long>();
        if (sourceCurrencyId != EurCurrencyId) currencyIds.Add(sourceCurrencyId);
        if (targetCurrencyId != EurCurrencyId) currencyIds.Add(targetCurrencyId);

        var rates = (await connection.QueryAsync<(long id, long currency_id, decimal rate_to_eur)>(
            sql, new { CurrencyIds = currencyIds })).ToList();

        decimal sourceRate = 1m; // EUR
        long exchangeRateId = 0;

        if (sourceCurrencyId != EurCurrencyId)
        {
            var sourceRow = rates.FirstOrDefault(r => r.currency_id == sourceCurrencyId);
            if (sourceRow == default)
                throw new InvalidOperationException($"No active exchange rate for currency {sourceCurrencyId}");
            sourceRate = sourceRow.rate_to_eur;
            exchangeRateId = sourceRow.id;
        }

        decimal targetRate = 1m; // EUR
        if (targetCurrencyId != EurCurrencyId)
        {
            var targetRow = rates.FirstOrDefault(r => r.currency_id == targetCurrencyId);
            if (targetRow == default)
                throw new InvalidOperationException($"No active exchange rate for currency {targetCurrencyId}");
            targetRate = targetRow.rate_to_eur;
            if (exchangeRateId == 0)
                exchangeRateId = targetRow.id;
        }

        // Convert: source → EUR → target
        // amount_in_eur = amount * source_rate_to_eur
        // result = amount_in_eur / target_rate_to_eur
        var converted = amount * sourceRate / targetRate;
        return (Math.Round(converted, 2), exchangeRateId);
    }
}
