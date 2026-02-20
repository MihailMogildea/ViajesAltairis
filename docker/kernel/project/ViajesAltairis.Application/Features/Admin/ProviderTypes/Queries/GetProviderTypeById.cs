using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.ProviderTypes.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.ProviderTypes.Queries;

public record GetProviderTypeByIdQuery(long Id) : IRequest<ProviderTypeDto?>;

public class GetProviderTypeByIdHandler : IRequestHandler<GetProviderTypeByIdQuery, ProviderTypeDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetProviderTypeByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<ProviderTypeDto?> Handle(GetProviderTypeByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<ProviderTypeDto>(
            "SELECT id AS Id, name AS Name, created_at AS CreatedAt FROM provider_type WHERE id = @Id",
            new { request.Id });
    }
}
