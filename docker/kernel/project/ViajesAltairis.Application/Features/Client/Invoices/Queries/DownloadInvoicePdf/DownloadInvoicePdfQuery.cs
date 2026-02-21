using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Invoices.Queries.DownloadInvoicePdf;

public class DownloadInvoicePdfQuery : IRequest<DownloadInvoicePdfResponse?>
{
    public long InvoiceId { get; set; }
}

public record DownloadInvoicePdfResponse(byte[] PdfBytes, string FileName);

public class DownloadInvoicePdfHandler : IRequestHandler<DownloadInvoicePdfQuery, DownloadInvoicePdfResponse?>
{
    private readonly IReservationApiClient _reservationApi;
    private readonly ICurrentUserService _currentUser;

    public DownloadInvoicePdfHandler(IReservationApiClient reservationApi, ICurrentUserService currentUser)
    {
        _reservationApi = reservationApi;
        _currentUser = currentUser;
    }

    public async Task<DownloadInvoicePdfResponse?> Handle(DownloadInvoicePdfQuery request, CancellationToken cancellationToken)
    {
        var pdfBytes = await _reservationApi.GetInvoicePdfAsync(
            request.InvoiceId, _currentUser.UserId!.Value, _currentUser.LanguageId, cancellationToken);

        if (pdfBytes is null)
            return null;

        return new DownloadInvoicePdfResponse(pdfBytes, $"invoice-{request.InvoiceId}.pdf");
    }
}
