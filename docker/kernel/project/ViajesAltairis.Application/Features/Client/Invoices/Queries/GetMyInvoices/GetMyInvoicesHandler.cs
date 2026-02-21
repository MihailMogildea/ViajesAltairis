using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Invoices.Queries.GetMyInvoices;

public class GetMyInvoicesHandler : IRequestHandler<GetMyInvoicesQuery, GetMyInvoicesResponse>
{
    private readonly IReservationApiClient _reservationApi;
    private readonly ICurrentUserService _currentUser;
    private readonly ITranslationService _translationService;

    public GetMyInvoicesHandler(
        IReservationApiClient reservationApi,
        ICurrentUserService currentUser,
        ITranslationService translationService)
    {
        _reservationApi = reservationApi;
        _currentUser = currentUser;
        _translationService = translationService;
    }

    public async Task<GetMyInvoicesResponse> Handle(GetMyInvoicesQuery request, CancellationToken cancellationToken)
    {
        var result = await _reservationApi.GetInvoicesByUserAsync(
            _currentUser.UserId!.Value, request.Page, request.PageSize, cancellationToken);

        var invoices = result.Invoices.Select(i => new InvoiceSummaryDto
        {
            Id = i.Id,
            InvoiceNumber = i.InvoiceNumber,
            Status = i.Status,
            StatusId = i.StatusId,
            TotalAmount = i.TotalAmount,
            Currency = i.Currency,
            IssuedAt = i.IssuedAt
        }).ToList();

        // Translate invoice status names
        var langId = _currentUser.LanguageId;
        var statusIds = invoices.Select(i => i.StatusId).Distinct().ToList();
        if (statusIds.Count > 0)
        {
            var statusNames = await _translationService.ResolveAsync(
                "invoice_status", statusIds, langId, "name", cancellationToken);
            foreach (var inv in invoices)
                if (statusNames.TryGetValue(inv.StatusId, out var name))
                    inv.Status = name;
        }

        return new GetMyInvoicesResponse
        {
            Invoices = invoices,
            TotalCount = result.TotalCount
        };
    }
}
