namespace ViajesAltairis.Application.Features.Admin.AuditLogs.Dtos;

public record AuditLogDto(
    long Id,
    long? UserId,
    string EntityType,
    long EntityId,
    string Action,
    string? OldValues,
    string? NewValues,
    DateTime CreatedAt);
