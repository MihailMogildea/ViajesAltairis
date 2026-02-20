namespace ViajesAltairis.Domain.Entities;

public class SubscriptionType : AuditableEntity
{
    public string Name { get; set; } = null!;
    public decimal PricePerMonth { get; set; }
    public decimal Discount { get; set; }
    public long CurrencyId { get; set; }
    public bool Enabled { get; set; }

    public Currency Currency { get; set; } = null!;
    public ICollection<UserSubscription> UserSubscriptions { get; set; } = [];
}
