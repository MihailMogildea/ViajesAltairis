using System.Dynamic;
using System.Net;
using System.Text.Json;
using ViajesAltairis.Providers.Api.Tests.Fixtures;

namespace ViajesAltairis.Providers.Api.Tests.Tests;

public class RoomTypesEndpointTests : IClassFixture<ProvidersApiFactory>
{
    private readonly HttpClient _client;
    private readonly ProvidersApiFactory _factory;

    public RoomTypesEndpointTests(ProvidersApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsRoomTypesWithBoards()
    {
        dynamic rt1 = new ExpandoObject();
        rt1.id = 10L; rt1.room_type = "double"; rt1.capacity = 2; rt1.quantity = 5; rt1.price_per_night = 120.00m; rt1.enabled = true;
        dynamic rt2 = new ExpandoObject();
        rt2.id = 11L; rt2.room_type = "suite"; rt2.capacity = 4; rt2.quantity = 2; rt2.price_per_night = 280.00m; rt2.enabled = true;

        _factory.MockHotelSyncRepo.GetRoomTypesForHotelProviderAsync(1, 1)
            .Returns(new List<dynamic> { rt1, rt2 }.AsEnumerable());

        dynamic b1 = new ExpandoObject(); b1.board_type = "room_only"; b1.price_per_night = 0.00m; b1.enabled = true;
        dynamic b2 = new ExpandoObject(); b2.board_type = "half_board"; b2.price_per_night = 25.00m; b2.enabled = true;

        _factory.MockHotelSyncRepo.GetBoardsForRoomTypeAsync(10)
            .Returns(new List<dynamic> { b1, b2 }.AsEnumerable());
        _factory.MockHotelSyncRepo.GetBoardsForRoomTypeAsync(11)
            .Returns(new List<dynamic> { b1 }.AsEnumerable());

        var response = await _client.GetAsync("/api/roomtypes?hotelId=1&providerId=1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        json.RootElement.GetArrayLength().Should().Be(2);
        json.RootElement[0].GetProperty("boards").GetArrayLength().Should().Be(2);
        json.RootElement[1].GetProperty("boards").GetArrayLength().Should().Be(1);
    }

    [Fact]
    public async Task Get_NoData_ReturnsEmpty()
    {
        _factory.MockHotelSyncRepo.GetRoomTypesForHotelProviderAsync(1, 999)
            .Returns(Enumerable.Empty<dynamic>());

        var response = await _client.GetAsync("/api/roomtypes?hotelId=1&providerId=999");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        json.RootElement.GetArrayLength().Should().Be(0);
    }
}
