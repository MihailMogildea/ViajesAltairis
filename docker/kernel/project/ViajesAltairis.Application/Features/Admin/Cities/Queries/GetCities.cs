using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Cities.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Cities.Queries;

public record GetCitiesQuery : IRequest<IEnumerable<CityDto>>;

public class GetCitiesHandler : IRequestHandler<GetCitiesQuery, IEnumerable<CityDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetCitiesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<CityDto>> Handle(GetCitiesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<CityDto>(
            "SELECT id AS Id, administrative_division_id AS AdministrativeDivisionId, name AS Name, enabled AS Enabled, created_at AS CreatedAt FROM city ORDER BY name");
    }
}
