using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Providers.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Providers.Queries;

public record GetProviderByIdQuery(long Id) : IRequest<ProviderDto?>;

public class GetProviderByIdHandler : IRequestHandler<GetProviderByIdQuery, ProviderDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetProviderByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<ProviderDto?> Handle(GetProviderByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<ProviderDto>(
            "SELECT id AS Id, type_id AS TypeId, currency_id AS CurrencyId, name AS Name, api_url AS ApiUrl, api_username AS ApiUsername, margin AS Margin, enabled AS Enabled, sync_status AS SyncStatus, last_synced_at AS LastSyncedAt, created_at AS CreatedAt FROM provider WHERE id = @Id",
            new { request.Id });
    }
}
