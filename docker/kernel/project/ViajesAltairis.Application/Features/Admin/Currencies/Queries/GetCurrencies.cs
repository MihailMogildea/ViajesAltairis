using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Currencies.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Currencies.Queries;

public record GetCurrenciesQuery : IRequest<IEnumerable<CurrencyDto>>;

public class GetCurrenciesHandler : IRequestHandler<GetCurrenciesQuery, IEnumerable<CurrencyDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetCurrenciesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<CurrencyDto>> Handle(GetCurrenciesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<CurrencyDto>(
            "SELECT id AS Id, iso_code AS IsoCode, name AS Name, symbol AS Symbol, created_at AS CreatedAt FROM currency ORDER BY name");
    }
}
