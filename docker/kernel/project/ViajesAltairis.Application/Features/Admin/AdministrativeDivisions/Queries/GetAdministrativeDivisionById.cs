using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.AdministrativeDivisions.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.AdministrativeDivisions.Queries;

public record GetAdministrativeDivisionByIdQuery(long Id) : IRequest<AdministrativeDivisionDto?>;

public class GetAdministrativeDivisionByIdHandler : IRequestHandler<GetAdministrativeDivisionByIdQuery, AdministrativeDivisionDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetAdministrativeDivisionByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<AdministrativeDivisionDto?> Handle(GetAdministrativeDivisionByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<AdministrativeDivisionDto>(
            "SELECT id AS Id, country_id AS CountryId, parent_id AS ParentId, name AS Name, type_id AS TypeId, level AS Level, enabled AS Enabled, created_at AS CreatedAt FROM administrative_division WHERE id = @Id",
            new { request.Id });
    }
}
