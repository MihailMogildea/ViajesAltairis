namespace ViajesAltairis.Application.Interfaces;

public interface IAuditService
{
    Task LogAsync(long? userId, string entityType, long entityId, string action, string? oldValues = null, string? newValues = null, CancellationToken cancellationToken = default);
}
