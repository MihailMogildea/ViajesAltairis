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
            @"SELECT a.id AS Id, a.user_id AS UserId, u.email AS UserEmail,
                     a.entity_type AS EntityType, a.entity_id AS EntityId,
                     a.action AS Action, a.old_values AS OldValues, a.new_values AS NewValues, a.created_at AS CreatedAt
              FROM audit_log a
              LEFT JOIN user u ON u.id = a.user_id
              ORDER BY a.created_at DESC");
    }
}
