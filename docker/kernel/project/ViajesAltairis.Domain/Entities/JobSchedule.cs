namespace ViajesAltairis.Domain.Entities;

public class JobSchedule : AuditableEntity
{
    public string JobKey { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string CronExpression { get; set; } = null!;
    public bool Enabled { get; set; }
    public DateTime? LastExecutedAt { get; set; }
}
