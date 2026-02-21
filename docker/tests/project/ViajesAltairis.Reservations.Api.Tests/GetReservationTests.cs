using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Reservations.Api.Tests.Fixtures;
using ViajesAltairis.Reservations.Api.Tests.Helpers;

namespace ViajesAltairis.Reservations.Api.Tests;

public class GetReservationTests : IntegrationTestBase
{
    public GetReservationTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetById_Found_Returns200()
    {
        var helper = new DapperMockHelper();
        // Main reservation query
        helper.EnqueueSingleRow(
            ("id", (long)1), ("booked_by_user_id", (long)1),
            ("owner_user_id", (object)DBNull.Value),
            ("status", "Confirmed"), ("created_at", DateTime.UtcNow),
            ("total_price", 400m), ("discount_amount", 20m),
            ("currency_code", "EUR"), ("exchange_rate", 1.0m),
            ("promo_code", (object)DBNull.Value));
        // Lines query — one line
        helper.EnqueueMultiRow(
            ["id", "hotel_name", "room_type", "board_type", "check_in", "check_out", "guest_count", "line_total"],
            new object[] { (long)10, "Hotel Test", "Double", "Half Board",
                DateTime.UtcNow.AddDays(30),
                DateTime.UtcNow.AddDays(33),
                2, 400m });
        // Guests for line 10
        helper.EnqueueMultiRow(
            ["id", "first_name", "last_name"],
            new object[] { (long)100, "Alice", "Wonder" });

        Factory.SetupDapperConnection(helper);

        var response = await GetAsync("/api/reservations/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadJsonAsync<ReservationDetailResult>(response);
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Lines.Should().HaveCount(1);
        result.Lines[0].Guests.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        var helper = new DapperMockHelper();
        helper.EnqueueEmptyQuery("id", "booked_by_user_id", "owner_user_id", "status", "created_at",
            "total_price", "discount_amount", "currency_code", "exchange_rate", "promo_code");
        Factory.SetupDapperConnection(helper);

        var response = await GetAsync("/api/reservations/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByUser_Returns200_PaginatedList()
    {
        var helper = new DapperMockHelper();
        // Count query
        helper.EnqueueScalar(2);
        // List query
        helper.EnqueueMultiRow(
            ["id", "status", "created_at", "total_price", "currency_code", "line_count"],
            new object[] { (long)1, "Confirmed", DateTime.UtcNow, 400m, "EUR", 2 },
            new object[] { (long)2, "Draft", DateTime.UtcNow, 200m, "EUR", 1 });
        Factory.SetupDapperConnection(helper);

        var response = await GetAsync("/api/reservations?userId=1&page=1&pageSize=20");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadJsonAsync<ReservationListResult>(response);
        result.Should().NotBeNull();
        result!.TotalCount.Should().Be(2);
        result.Reservations.Should().HaveCount(2);
    }
}
