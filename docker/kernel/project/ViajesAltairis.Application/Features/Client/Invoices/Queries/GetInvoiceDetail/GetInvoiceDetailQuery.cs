using MediatR;

namespace ViajesAltairis.Application.Features.Client.Invoices.Queries.GetInvoiceDetail;

public class GetInvoiceDetailQuery : IRequest<GetInvoiceDetailResponse>
{
    public long InvoiceId { get; set; }
}

public class GetInvoiceDetailResponse
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
    public long ReservationId { get; set; }
}
