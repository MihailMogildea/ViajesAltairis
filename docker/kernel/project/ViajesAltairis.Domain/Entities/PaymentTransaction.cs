namespace ViajesAltairis.Domain.Entities;

public class PaymentTransaction : AuditableEntity
{
    public long ReservationId { get; set; }
    public long PaymentMethodId { get; set; }
    public string TransactionReference { get; set; } = null!;
    public decimal Amount { get; set; }
    public long CurrencyId { get; set; }
    public long ExchangeRateId { get; set; }
    public long StatusId { get; set; }

    public Reservation Reservation { get; set; } = null!;
    public PaymentMethod PaymentMethod { get; set; } = null!;
    public Currency Currency { get; set; } = null!;
    public ExchangeRate ExchangeRate { get; set; } = null!;
    public ICollection<PaymentTransactionFee> PaymentTransactionFees { get; set; } = [];
}
