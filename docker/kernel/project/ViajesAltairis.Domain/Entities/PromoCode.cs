namespace ViajesAltairis.Domain.Entities;

public class PromoCode : AuditableEntity
{
    public string Code { get; set; } = null!;
    public decimal? DiscountPercentage { get; set; }
    public decimal? DiscountAmount { get; set; }
    public long? CurrencyId { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly ValidTo { get; set; }
    public int? MaxUses { get; set; }
    public int CurrentUses { get; set; }
    public bool Enabled { get; set; }

    public Currency? Currency { get; set; }
    public ICollection<Reservation> Reservations { get; set; } = [];
}
