namespace ViajesAltairis.Domain.Entities;

public class EmailTemplate
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<NotificationLog> NotificationLogs { get; set; } = [];
}
