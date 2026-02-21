namespace ViajesAltairis.Application.Features.Admin.Invoices.Dtos;

public class InvoiceDto
{
    public long Id { get; init; }
    public string InvoiceNumber { get; init; } = null!;
    public long StatusId { get; init; }
    public long ReservationId { get; init; }
    public long? BusinessPartnerId { get; init; }
    public decimal Subtotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public DateOnly PeriodStart { get; init; }
    public DateOnly PeriodEnd { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
