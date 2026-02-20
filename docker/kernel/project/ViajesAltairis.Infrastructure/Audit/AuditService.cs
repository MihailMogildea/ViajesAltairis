using Microsoft.EntityFrameworkCore;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Infrastructure.Audit;

public class AuditService : IAuditService
{
    private readonly DbContext _context;

    public AuditService(DbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(long? userId, string entityType, long entityId, string action, string? oldValues = null, string? newValues = null, CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog
        {
            UserId = userId,
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            OldValues = oldValues,
            NewValues = newValues
        };

        _context.Set<AuditLog>().Add(auditLog);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
