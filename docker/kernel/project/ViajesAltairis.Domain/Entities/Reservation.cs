namespace ViajesAltairis.Domain.Entities;

public class Reservation : AuditableEntity
{
    public string ReservationCode { get; set; } = null!;
    public long StatusId { get; set; }
    public long BookedByUserId { get; set; }
    public long? OwnerUserId { get; set; }
    public string OwnerFirstName { get; set; } = null!;
    public string OwnerLastName { get; set; } = null!;
    public string? OwnerEmail { get; set; }
    public string? OwnerPhone { get; set; }
    public string? OwnerTaxId { get; set; }
    public string? OwnerAddress { get; set; }
    public string? OwnerCity { get; set; }
    public string? OwnerPostalCode { get; set; }
    public string? OwnerCountry { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal MarginAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalPrice { get; set; }
    public long CurrencyId { get; set; }
    public long ExchangeRateId { get; set; }
    public long? PromoCodeId { get; set; }
    public string? Notes { get; set; }

    public ReservationStatus Status { get; set; } = null!;
    public User BookedByUser { get; set; } = null!;
    public User? OwnerUser { get; set; }
    public Currency Currency { get; set; } = null!;
    public ExchangeRate ExchangeRate { get; set; } = null!;
    public PromoCode? PromoCode { get; set; }
    public ICollection<ReservationLine> ReservationLines { get; set; } = [];
    public ICollection<Invoice> Invoices { get; set; } = [];
    public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = [];
    public Cancellation? Cancellation { get; set; }
    public Review? Review { get; set; }
}
