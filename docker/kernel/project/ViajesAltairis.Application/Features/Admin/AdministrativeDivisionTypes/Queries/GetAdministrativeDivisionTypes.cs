using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.AdministrativeDivisionTypes.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.AdministrativeDivisionTypes.Queries;

public record GetAdministrativeDivisionTypesQuery : IRequest<IEnumerable<AdministrativeDivisionTypeDto>>;

public class GetAdministrativeDivisionTypesHandler : IRequestHandler<GetAdministrativeDivisionTypesQuery, IEnumerable<AdministrativeDivisionTypeDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetAdministrativeDivisionTypesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<AdministrativeDivisionTypeDto>> Handle(GetAdministrativeDivisionTypesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<AdministrativeDivisionTypeDto>(
            "SELECT id AS Id, name AS Name, created_at AS CreatedAt FROM administrative_division_type ORDER BY name");
    }
}
