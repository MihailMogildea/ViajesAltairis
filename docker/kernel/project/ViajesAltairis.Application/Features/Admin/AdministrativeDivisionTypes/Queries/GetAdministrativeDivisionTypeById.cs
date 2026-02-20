using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.AdministrativeDivisionTypes.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.AdministrativeDivisionTypes.Queries;

public record GetAdministrativeDivisionTypeByIdQuery(long Id) : IRequest<AdministrativeDivisionTypeDto?>;

public class GetAdministrativeDivisionTypeByIdHandler : IRequestHandler<GetAdministrativeDivisionTypeByIdQuery, AdministrativeDivisionTypeDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetAdministrativeDivisionTypeByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<AdministrativeDivisionTypeDto?> Handle(GetAdministrativeDivisionTypeByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<AdministrativeDivisionTypeDto>(
            "SELECT id AS Id, name AS Name, created_at AS CreatedAt FROM administrative_division_type WHERE id = @Id",
            new { request.Id });
    }
}
