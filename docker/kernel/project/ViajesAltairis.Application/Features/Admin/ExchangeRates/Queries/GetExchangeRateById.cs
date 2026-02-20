using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.ExchangeRates.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.ExchangeRates.Queries;

public record GetExchangeRateByIdQuery(long Id) : IRequest<ExchangeRateDto?>;

public class GetExchangeRateByIdHandler : IRequestHandler<GetExchangeRateByIdQuery, ExchangeRateDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetExchangeRateByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<ExchangeRateDto?> Handle(GetExchangeRateByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<ExchangeRateDto>(
            "SELECT id AS Id, currency_id AS CurrencyId, rate_to_eur AS RateToEur, valid_from AS ValidFrom, valid_to AS ValidTo, created_at AS CreatedAt FROM exchange_rate WHERE id = @Id",
            new { request.Id });
    }
}
