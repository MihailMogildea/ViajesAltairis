using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.AuditLogs.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.AuditLogs.Queries;

public record GetAuditLogsQuery : IRequest<IEnumerable<AuditLogDto>>;

public class GetAuditLogsHandler : IRequestHandler<GetAuditLogsQuery, IEnumerable<AuditLogDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetAuditLogsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<AuditLogDto>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<AuditLogDto>(
            @"SELECT id AS Id, user_id AS UserId, entity_type AS EntityType, entity_id AS EntityId,
                     action AS Action, old_values AS OldValues, new_values AS NewValues, created_at AS CreatedAt
              FROM audit_log ORDER BY created_at DESC");
    }
}
