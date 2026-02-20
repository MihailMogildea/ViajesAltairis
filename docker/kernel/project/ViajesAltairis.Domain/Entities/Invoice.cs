namespace ViajesAltairis.Domain.Entities;

public class Invoice : AuditableEntity
{
    public string InvoiceNumber { get; set; } = null!;
    public long StatusId { get; set; }
    public long ReservationId { get; set; }
    public long? BusinessPartnerId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }

    public InvoiceStatus Status { get; set; } = null!;
    public Reservation Reservation { get; set; } = null!;
    public BusinessPartner? BusinessPartner { get; set; }
}
