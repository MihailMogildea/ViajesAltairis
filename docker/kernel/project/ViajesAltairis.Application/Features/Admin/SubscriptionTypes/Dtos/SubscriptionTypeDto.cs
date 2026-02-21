namespace ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Dtos;

public class SubscriptionTypeDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PricePerMonth { get; set; }
    public decimal Discount { get; set; }
    public long CurrencyId { get; set; }
    public bool Enabled { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
