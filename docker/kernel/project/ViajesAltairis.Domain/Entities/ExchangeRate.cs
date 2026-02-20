namespace ViajesAltairis.Domain.Entities;

public class ExchangeRate : BaseEntity
{
    public long CurrencyId { get; set; }
    public decimal RateToEur { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }

    public Currency Currency { get; set; } = null!;
}
