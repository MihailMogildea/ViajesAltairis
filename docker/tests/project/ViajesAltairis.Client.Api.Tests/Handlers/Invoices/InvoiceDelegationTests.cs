using FluentAssertions;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Invoices.Queries.GetInvoiceDetail;
using ViajesAltairis.Application.Features.Client.Invoices.Queries.GetMyInvoices;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Client.Api.Tests.Handlers.Invoices;

public class InvoiceDelegationTests
{
    private readonly IReservationApiClient _reservationApi = Substitute.For<IReservationApiClient>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ITranslationService _translationService = Substitute.For<ITranslationService>();

    public InvoiceDelegationTests()
    {
        _currentUser.UserId.Returns(8L);
        _currentUser.LanguageId.Returns(1L);
    }

    [Fact]
    public async Task GetMyInvoices_DelegatesToReservationApi()
    {
        _reservationApi.GetInvoicesByUserAsync(8, 1, 10, Arg.Any<CancellationToken>())
            .Returns(new InvoiceListResult(
            [
                new InvoiceSummaryResult(1, "INV-001", 2, "paid", 500, "EUR", DateTime.UtcNow)
            ], 1));

        _translationService.ResolveAsync("invoice_status", Arg.Any<List<long>>(), 1, "name", Arg.Any<CancellationToken>())
            .Returns(new Dictionary<long, string> { { 2, "Paid" } });

        var handler = new GetMyInvoicesHandler(_reservationApi, _currentUser, _translationService);
        var result = await handler.Handle(new GetMyInvoicesQuery { Page = 1, PageSize = 10 }, CancellationToken.None);

        result.Invoices.Should().HaveCount(1);
        result.Invoices.First().InvoiceNumber.Should().Be("INV-001");
        result.Invoices.First().Status.Should().Be("Paid");
    }

    [Fact]
    public async Task GetInvoiceDetail_ReturnsDetail()
    {
        _reservationApi.GetInvoiceByIdAsync(1, 8, Arg.Any<CancellationToken>())
            .Returns(new InvoiceDetailResult(1, "INV-001", 2, "paid", 400, 100, 500, "EUR", 1.0m, DateTime.UtcNow, DateTime.UtcNow, 100));

        _translationService.ResolveAsync("invoice_status", Arg.Any<IEnumerable<long>>(), 1, "name", Arg.Any<CancellationToken>())
            .Returns(new Dictionary<long, string> { { 2, "Paid" } });

        var handler = new GetInvoiceDetailHandler(_reservationApi, _currentUser, _translationService);
        var result = await handler.Handle(new GetInvoiceDetailQuery { InvoiceId = 1 }, CancellationToken.None);

        result.InvoiceNumber.Should().Be("INV-001");
        result.TotalAmount.Should().Be(500);
        result.Status.Should().Be("Paid");
    }
}
