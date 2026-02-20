using MediatR;

namespace ViajesAltairis.Application.Features.Client.Invoices.Queries.GetMyInvoices;

public class GetMyInvoicesQuery : IRequest<GetMyInvoicesResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetMyInvoicesResponse
{
    public List<InvoiceSummaryDto> Invoices { get; set; } = new();
    public int TotalCount { get; set; }
}

public class InvoiceSummaryDto
{
    public long Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
}
