using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.AuditLogs.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.AuditLogs.Queries;

public record GetAuditLogByIdQuery(long Id) : IRequest<AuditLogDto?>;

public class GetAuditLogByIdHandler : IRequestHandler<GetAuditLogByIdQuery, AuditLogDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetAuditLogByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<AuditLogDto?> Handle(GetAuditLogByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<AuditLogDto>(
            @"SELECT id AS Id, user_id AS UserId, entity_type AS EntityType, entity_id AS EntityId,
                     action AS Action, old_values AS OldValues, new_values AS NewValues, created_at AS CreatedAt
              FROM audit_log WHERE id = @Id",
            new { request.Id });
    }
}
