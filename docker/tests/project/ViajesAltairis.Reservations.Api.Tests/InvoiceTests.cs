using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Reservations.Api.Tests.Fixtures;
using ViajesAltairis.Reservations.Api.Tests.Helpers;

namespace ViajesAltairis.Reservations.Api.Tests;

public class InvoiceTests : IntegrationTestBase
{
    public InvoiceTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetInvoicesByUser_Returns200()
    {
        var helper = new DapperMockHelper();
        // Count
        helper.EnqueueScalar(1);
        // List
        helper.EnqueueMultiRow(
            ["id", "invoice_number", "status", "total_amount", "currency", "created_at"],
            new object[] { (long)1, "INV-2024-001", "Issued", 400m, "EUR", DateTime.UtcNow });
        Factory.SetupDapperConnection(helper);

        var response = await GetAsync("/api/invoices?userId=1&page=1&pageSize=20");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadJsonAsync<InvoiceListResult>(response);
        result.Should().NotBeNull();
        result!.TotalCount.Should().Be(1);
        result.Invoices.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetInvoiceById_Found_Returns200()
    {
        var helper = new DapperMockHelper();
        helper.EnqueueSingleRow(
            ("id", (long)1), ("invoice_number", "INV-2024-001"),
            ("status", "Issued"), ("subtotal", 360m),
            ("tax_amount", 40m), ("total_amount", 400m),
            ("currency", "EUR"), ("exchange_rate_to_eur", 1.0m),
            ("created_at", DateTime.UtcNow),
            ("updated_at", (object)DBNull.Value),
            ("reservation_id", (long)1));
        Factory.SetupDapperConnection(helper);

        var response = await GetAsync("/api/invoices/1?userId=1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadJsonAsync<InvoiceDetailResult>(response);
        result.Should().NotBeNull();
        result!.InvoiceNumber.Should().Be("INV-2024-001");
        result.TotalAmount.Should().Be(400m);
    }

    [Fact]
    public async Task GetInvoiceById_WrongUser_Returns404()
    {
        var helper = new DapperMockHelper();
        helper.EnqueueEmptyQuery("id", "invoice_number", "status", "subtotal",
            "tax_amount", "total_amount", "currency", "exchange_rate_to_eur",
            "created_at", "updated_at", "reservation_id");
        Factory.SetupDapperConnection(helper);

        var response = await GetAsync("/api/invoices/1?userId=999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
