namespace ViajesAltairis.Domain.Entities;

public class UserSubscription : AuditableEntity
{
    public long UserId { get; set; }
    public long SubscriptionTypeId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool Active { get; set; }

    public User User { get; set; } = null!;
    public SubscriptionType SubscriptionType { get; set; } = null!;
}
