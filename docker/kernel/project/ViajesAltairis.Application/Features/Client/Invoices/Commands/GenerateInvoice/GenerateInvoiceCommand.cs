using MediatR;

namespace ViajesAltairis.Application.Features.Client.Invoices.Commands.GenerateInvoice;

public class GenerateInvoiceCommand : IRequest<GenerateInvoiceResponse>
{
    public long ReservationId { get; set; }
}

public class GenerateInvoiceResponse
{
    public long Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal ExchangeRateToEur { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public long InvoiceReservationId { get; set; }
}
