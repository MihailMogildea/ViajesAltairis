namespace ViajesAltairis.Application.Features.Admin.AuditLogs.Dtos;

public class AuditLogDto
{
    public long Id { get; init; }
    public long? UserId { get; init; }
    public string? UserEmail { get; init; }
    public string EntityType { get; init; } = null!;
    public long EntityId { get; init; }
    public string Action { get; init; } = null!;
    public string? OldValues { get; init; }
    public string? NewValues { get; init; }
    public DateTime CreatedAt { get; init; }
}
