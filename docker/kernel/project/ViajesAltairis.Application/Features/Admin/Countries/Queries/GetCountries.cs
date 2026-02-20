using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Countries.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Countries.Queries;

public record GetCountriesQuery : IRequest<IEnumerable<CountryDto>>;

public class GetCountriesHandler : IRequestHandler<GetCountriesQuery, IEnumerable<CountryDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetCountriesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<CountryDto>> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<CountryDto>(
            "SELECT id AS Id, iso_code AS IsoCode, name AS Name, currency_id AS CurrencyId, enabled AS Enabled, created_at AS CreatedAt FROM country ORDER BY name");
    }
}
