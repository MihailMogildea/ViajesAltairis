using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Countries.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Countries.Queries;

public record GetCountryByIdQuery(long Id) : IRequest<CountryDto?>;

public class GetCountryByIdHandler : IRequestHandler<GetCountryByIdQuery, CountryDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetCountryByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<CountryDto?> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<CountryDto>(
            "SELECT id AS Id, iso_code AS IsoCode, name AS Name, currency_id AS CurrencyId, enabled AS Enabled, created_at AS CreatedAt FROM country WHERE id = @Id",
            new { request.Id });
    }
}
