namespace ViajesAltairis.Domain.Entities;

public class PaymentTransactionFee : BaseEntity
{
    public long PaymentTransactionId { get; set; }
    public string FeeType { get; set; } = null!;
    public decimal FeeAmount { get; set; }
    public decimal? FeePercentage { get; set; }
    public decimal? FixedFeeAmount { get; set; }
    public long CurrencyId { get; set; }
    public string? Description { get; set; }

    public PaymentTransaction PaymentTransaction { get; set; } = null!;
    public Currency Currency { get; set; } = null!;
}
