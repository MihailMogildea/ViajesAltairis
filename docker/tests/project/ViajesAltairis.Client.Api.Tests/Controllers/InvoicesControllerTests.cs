using System.Net;
using FluentAssertions;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ViajesAltairis.Application.Features.Client.Invoices.Queries.GetInvoiceDetail;
using ViajesAltairis.Application.Features.Client.Invoices.Queries.GetMyInvoices;
using ViajesAltairis.Client.Api.Tests.Fixtures;
using ViajesAltairis.Client.Api.Tests.Helpers;

namespace ViajesAltairis.Client.Api.Tests.Controllers;

public class InvoicesControllerTests : IClassFixture<ClientApiFactory>
{
    private readonly ClientApiFactory _factory;

    public InvoicesControllerTests(ClientApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetMyInvoices_Returns401_WhenNoToken()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/invoices");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMyInvoices_ReturnsOk_WhenAuthenticated()
    {
        var client = _factory.CreateClient().WithClientAuth();
        _factory.MockMediator
            .Send(Arg.Any<GetMyInvoicesQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetMyInvoicesResponse { Invoices = [], TotalCount = 0 });

        var response = await client.GetAsync("/api/invoices");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDetail_ReturnsOk_WhenAuthenticated()
    {
        var client = _factory.CreateClient().WithClientAuth();
        _factory.MockMediator
            .Send(Arg.Any<GetInvoiceDetailQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetInvoiceDetailResponse { Id = 1, InvoiceNumber = "INV-001", Status = "paid", TotalAmount = 500m, Currency = "EUR" });

        var response = await client.GetAsync("/api/invoices/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDetail_Returns404_WhenNotFound()
    {
        var client = _factory.CreateClient().WithClientAuth();
        _factory.MockMediator
            .Send(Arg.Any<GetInvoiceDetailQuery>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new KeyNotFoundException("Invoice 999 not found."));

        var response = await client.GetAsync("/api/invoices/999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
