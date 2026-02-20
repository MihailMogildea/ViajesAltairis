using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.AdministrativeDivisions.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.AdministrativeDivisions.Queries;

public record GetAdministrativeDivisionsQuery : IRequest<IEnumerable<AdministrativeDivisionDto>>;

public class GetAdministrativeDivisionsHandler : IRequestHandler<GetAdministrativeDivisionsQuery, IEnumerable<AdministrativeDivisionDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetAdministrativeDivisionsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<AdministrativeDivisionDto>> Handle(GetAdministrativeDivisionsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<AdministrativeDivisionDto>(
            "SELECT id AS Id, country_id AS CountryId, parent_id AS ParentId, name AS Name, type_id AS TypeId, level AS Level, enabled AS Enabled, created_at AS CreatedAt FROM administrative_division ORDER BY name");
    }
}
