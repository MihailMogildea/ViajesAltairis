namespace ViajesAltairis.Domain.Entities;

public class AuditLog : BaseEntity
{
    public long? UserId { get; set; }
    public string EntityType { get; set; } = null!;
    public long EntityId { get; set; }
    public string Action { get; set; } = null!;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }

    public User? User { get; set; }
}
