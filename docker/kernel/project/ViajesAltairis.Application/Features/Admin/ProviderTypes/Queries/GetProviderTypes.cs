using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.ProviderTypes.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.ProviderTypes.Queries;

public record GetProviderTypesQuery : IRequest<IEnumerable<ProviderTypeDto>>;

public class GetProviderTypesHandler : IRequestHandler<GetProviderTypesQuery, IEnumerable<ProviderTypeDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetProviderTypesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<ProviderTypeDto>> Handle(GetProviderTypesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<ProviderTypeDto>(
            "SELECT id AS Id, name AS Name, created_at AS CreatedAt FROM provider_type ORDER BY name");
    }
}
