using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Reference.Queries.GetCurrencies;

public class GetCurrenciesHandler : IRequestHandler<GetCurrenciesQuery, GetCurrenciesResponse>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetCurrenciesHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<GetCurrenciesResponse> Handle(GetCurrenciesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT
                c.id AS Id,
                c.iso_code AS Code,
                c.name AS Name,
                c.symbol AS Symbol,
                er.rate_to_eur AS ExchangeRateToEur
            FROM currency c
            LEFT JOIN v_exchange_rate_current er ON er.currency_id = c.id
            ORDER BY c.iso_code
            """;

        var currencies = (await connection.QueryAsync<CurrencyDto>(sql)).ToList();

        return new GetCurrenciesResponse { Currencies = currencies };
    }
}
