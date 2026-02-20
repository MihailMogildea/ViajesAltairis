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

    public InvoiceDelegationTests()
    {
        _currentUser.UserId.Returns(8L);
    }

    [Fact]
    public async Task GetMyInvoices_DelegatesToReservationApi()
    {
        _reservationApi.GetInvoicesByUserAsync(8, 1, 10, Arg.Any<CancellationToken>())
            .Returns(new InvoiceListResult(
            [
                new InvoiceSummaryResult(1, "INV-001", "paid", 500, "EUR", DateTime.UtcNow)
            ], 1));

        var handler = new GetMyInvoicesHandler(_reservationApi, _currentUser);
        var result = await handler.Handle(new GetMyInvoicesQuery { Page = 1, PageSize = 10 }, CancellationToken.None);

        result.Invoices.Should().HaveCount(1);
        result.Invoices.First().InvoiceNumber.Should().Be("INV-001");
    }

    [Fact]
    public async Task GetInvoiceDetail_ReturnsDetail()
    {
        _reservationApi.GetInvoiceByIdAsync(1, 8, Arg.Any<CancellationToken>())
            .Returns(new InvoiceDetailResult(1, "INV-001", "paid", 400, 100, 500, "EUR", 1.0m, DateTime.UtcNow, DateTime.UtcNow, 100));

        var handler = new GetInvoiceDetailHandler(_reservationApi, _currentUser);
        var result = await handler.Handle(new GetInvoiceDetailQuery { InvoiceId = 1 }, CancellationToken.None);

        result.InvoiceNumber.Should().Be("INV-001");
        result.TotalAmount.Should().Be(500);
    }
}
