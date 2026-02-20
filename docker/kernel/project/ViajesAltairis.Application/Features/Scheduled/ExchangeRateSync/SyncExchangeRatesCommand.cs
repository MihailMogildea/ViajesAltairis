using System.Data;
using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Scheduled.ExchangeRateSync;

public record SyncExchangeRatesCommand : IRequest<int>;

public class SyncExchangeRatesHandler : IRequestHandler<SyncExchangeRatesCommand, int>
{
    private readonly IDbConnectionFactory _db;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IEcbRateParser _parser;
    private readonly ILogger<SyncExchangeRatesHandler> _logger;

    public SyncExchangeRatesHandler(IDbConnectionFactory db, IHttpClientFactory httpClientFactory, IEcbRateParser parser, ILogger<SyncExchangeRatesHandler> logger)
    {
        _db = db;
        _httpClientFactory = httpClientFactory;
        _parser = parser;
        _logger = logger;
    }

    public async Task<int> Handle(SyncExchangeRatesCommand request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();

        // Get all currencies except EUR (base currency)
        var currencies = (await connection.QueryAsync<(long Id, string IsoCode)>(
            "SELECT id AS Id, iso_code AS IsoCode FROM currency WHERE iso_code != 'EUR'"))
            .ToList();

        if (currencies.Count == 0)
        {
            _logger.LogWarning("No non-EUR currencies found, skipping sync");
            return 0;
        }

        // Build ECB API URL with all currency codes
        var codes = string.Join("+", currencies.Select(c => c.IsoCode));
        var url = $"https://data-api.ecb.europa.eu/service/data/EXR/D.{codes}.EUR.SP00.A?lastNObservations=1&format=csvdata";

        _logger.LogInformation("Fetching ECB rates for: {Codes}", codes);

        var client = _httpClientFactory.CreateClient();
        var csv = await client.GetStringAsync(url, cancellationToken);

        // Parse CSV using extracted service
        var rates = _parser.ParseRates(csv);

        if (rates.Count == 0)
        {
            _logger.LogWarning("ECB API returned no usable rate data");
            return 0;
        }

        // Update rates inside a transaction
        connection.Open();
        using var transaction = connection.BeginTransaction();

        int updated = 0;
        foreach (var currency in currencies)
        {
            if (!rates.TryGetValue(currency.IsoCode, out var ecbValue))
            {
                _logger.LogWarning("No ECB rate found for {Currency}", currency.IsoCode);
                continue;
            }

            var rateToEur = _parser.CalculateRateToEur(ecbValue);

            // Close current rate
            await connection.ExecuteAsync(
                "UPDATE exchange_rate SET valid_to = NOW() WHERE currency_id = @CurrencyId AND valid_to > NOW()",
                new { CurrencyId = currency.Id }, transaction);

            // Insert new rate
            await connection.ExecuteAsync(
                @"INSERT INTO exchange_rate (currency_id, rate_to_eur, valid_from, valid_to)
                  VALUES (@CurrencyId, @RateToEur, NOW(), '2099-12-31 23:59:59')",
                new { CurrencyId = currency.Id, RateToEur = rateToEur }, transaction);

            _logger.LogInformation("Updated {Currency}: ECB={EcbValue}, rate_to_eur={RateToEur}",
                currency.IsoCode, ecbValue, rateToEur);
            updated++;
        }

        transaction.Commit();
        return updated;
    }
}
