using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Cities.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Cities.Queries;

public record GetCityByIdQuery(long Id) : IRequest<CityDto?>;

public class GetCityByIdHandler : IRequestHandler<GetCityByIdQuery, CityDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetCityByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<CityDto?> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<CityDto>(
            "SELECT id AS Id, administrative_division_id AS AdministrativeDivisionId, name AS Name, enabled AS Enabled, created_at AS CreatedAt FROM city WHERE id = @Id",
            new { request.Id });
    }
}
