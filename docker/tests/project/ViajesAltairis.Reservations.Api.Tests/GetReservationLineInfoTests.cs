using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Reservations.Api.Tests.Fixtures;
using ViajesAltairis.Reservations.Api.Tests.Helpers;

namespace ViajesAltairis.Reservations.Api.Tests;

public class GetReservationLineInfoTests : IntegrationTestBase
{
    public GetReservationLineInfoTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetLineInfo_Found_Returns200()
    {
        var helper = new DapperMockHelper();
        helper.EnqueueSingleRow(
            ("reservation_line_id", (long)10),
            ("reservation_id", (long)1),
            ("hotel_id", (long)5),
            ("user_id", (long)1));
        Factory.SetupDapperConnection(helper);

        var response = await GetAsync("/api/reservations/lines/10/info");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadJsonAsync<ReservationLineInfoResult>(response);
        result.Should().NotBeNull();
        result!.ReservationLineId.Should().Be(10);
        result.HotelId.Should().Be(5);
    }

    [Fact]
    public async Task GetLineInfo_NotFound_Returns404()
    {
        var helper = new DapperMockHelper();
        helper.EnqueueEmptyQuery("reservation_line_id", "reservation_id", "hotel_id", "user_id");
        Factory.SetupDapperConnection(helper);

        var response = await GetAsync("/api/reservations/lines/999/info");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
