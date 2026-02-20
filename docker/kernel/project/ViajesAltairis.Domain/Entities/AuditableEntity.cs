namespace ViajesAltairis.Domain.Entities;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime UpdatedAt { get; set; }
}
