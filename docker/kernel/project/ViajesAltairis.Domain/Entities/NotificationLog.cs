namespace ViajesAltairis.Domain.Entities;

public class NotificationLog : BaseEntity
{
    public long UserId { get; set; }
    public long EmailTemplateId { get; set; }
    public string RecipientEmail { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;

    public User User { get; set; } = null!;
    public EmailTemplate EmailTemplate { get; set; } = null!;
}
