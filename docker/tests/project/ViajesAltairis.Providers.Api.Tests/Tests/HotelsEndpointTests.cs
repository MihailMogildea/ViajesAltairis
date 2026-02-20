using System.Dynamic;
using System.Net;
using System.Text.Json;
using NSubstitute;
using ViajesAltairis.Providers.Api.Tests.Fixtures;

namespace ViajesAltairis.Providers.Api.Tests.Tests;

public class HotelsEndpointTests : IClassFixture<ProvidersApiFactory>
{
    private readonly HttpClient _client;
    private readonly ProvidersApiFactory _factory;

    public HotelsEndpointTests(ProvidersApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Search_ReturnsHotels()
    {
        dynamic h1 = new ExpandoObject(); h1.id = 1L; h1.name = "Hotel Sol"; h1.stars = 4; h1.city_name = "Palma";
        dynamic h2 = new ExpandoObject(); h2.id = 2L; h2.name = "Hotel Luna"; h2.stars = 3; h2.city_name = "Barcelona";

        _factory.MockHotelSyncRepo.SearchHotelsAsync(null, null)
            .Returns(new List<dynamic> { h1, h2 }.AsEnumerable());

        var response = await _client.GetAsync("/api/hotels");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        json.RootElement.GetArrayLength().Should().Be(2);
    }

    [Fact]
    public async Task Search_WithFilters_PassesParams()
    {
        dynamic h1 = new ExpandoObject(); h1.id = 1L; h1.name = "Grand Hotel"; h1.stars = 5; h1.city_name = "Palma";

        _factory.MockHotelSyncRepo.SearchHotelsAsync("Palma", 5)
            .Returns(new List<dynamic> { h1 }.AsEnumerable());

        var response = await _client.GetAsync("/api/hotels?city=Palma&stars=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        json.RootElement.GetArrayLength().Should().Be(1);
        json.RootElement[0].GetProperty("name").GetString().Should().Be("Grand Hotel");
    }

    [Fact]
    public async Task GetById_Found_ReturnsHotel()
    {
        dynamic hotel = new ExpandoObject();
        hotel.id = 1L;
        hotel.name = "Hotel Altairis";
        hotel.stars = 5;
        hotel.city_name = "Palma";
        hotel.address = "Calle Mayor 1";

        SubstituteExtensions.Returns(_factory.MockHotelSyncRepo.GetHotelDetailAsync(1), (dynamic?)hotel);

        var response = await _client.GetAsync("/api/hotels/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        json.RootElement.GetProperty("name").GetString().Should().Be("Hotel Altairis");
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        // NSubstitute returns null by default for unconfigured args (id=99)
        var response = await _client.GetAsync("/api/hotels/99");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
