using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.ExchangeRates.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.ExchangeRates.Queries;

public record GetExchangeRatesQuery : IRequest<IEnumerable<ExchangeRateDto>>;

public class GetExchangeRatesHandler : IRequestHandler<GetExchangeRatesQuery, IEnumerable<ExchangeRateDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetExchangeRatesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<ExchangeRateDto>> Handle(GetExchangeRatesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<ExchangeRateDto>(
            "SELECT id AS Id, currency_id AS CurrencyId, rate_to_eur AS RateToEur, valid_from AS ValidFrom, valid_to AS ValidTo, created_at AS CreatedAt FROM exchange_rate ORDER BY valid_from DESC");
    }
}
