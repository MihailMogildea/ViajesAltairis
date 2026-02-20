namespace ViajesAltairis.Application.Features.Admin.Invoices.Dtos;

public record InvoiceDto(
    long Id,
    string InvoiceNumber,
    long StatusId,
    long ReservationId,
    long? BusinessPartnerId,
    decimal Subtotal,
    decimal TaxAmount,
    decimal DiscountAmount,
    decimal TotalAmount,
    DateOnly PeriodStart,
    DateOnly PeriodEnd,
    DateTime CreatedAt,
    DateTime UpdatedAt);
