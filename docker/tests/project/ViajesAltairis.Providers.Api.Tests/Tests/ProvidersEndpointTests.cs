using System.Dynamic;
using System.Net;
using System.Text.Json;
using NSubstitute;
using ViajesAltairis.Providers.Api.Tests.Fixtures;

namespace ViajesAltairis.Providers.Api.Tests.Tests;

public class ProvidersEndpointTests : IClassFixture<ProvidersApiFactory>
{
    private readonly HttpClient _client;
    private readonly ProvidersApiFactory _factory;

    public ProvidersEndpointTests(ProvidersApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsProviders()
    {
        dynamic p1 = new ExpandoObject(); p1.id = 1L; p1.name = "Internal"; p1.type = "internal";
        dynamic p2 = new ExpandoObject(); p2.id = 4L; p2.name = "HotelBeds"; p2.type = "external";
        dynamic p3 = new ExpandoObject(); p3.id = 5L; p3.name = "BookingDotCom"; p3.type = "external";

        _factory.MockProviderRepo.GetAllEnabledAsync()
            .Returns(new List<dynamic> { p1, p2, p3 }.AsEnumerable());

        var response = await _client.GetAsync("/api/providers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        json.RootElement.GetArrayLength().Should().Be(3);
    }

    [Fact]
    public async Task GetById_Found_ReturnsDetail()
    {
        dynamic provider = new ExpandoObject();
        provider.id = 6L;
        provider.name = "HotelBeds";
        provider.type = "external";
        provider.margin = 10.00m;
        provider.enabled = true;
        provider.currency_iso_code = "EUR";

        SubstituteExtensions.Returns(_factory.MockProviderRepo.GetByIdAsync(6), (dynamic?)provider);

        var response = await _client.GetAsync("/api/providers/6");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        json.RootElement.GetProperty("name").GetString().Should().Be("HotelBeds");
        json.RootElement.GetProperty("currency_iso_code").GetString().Should().Be("EUR");
    }

    [Fact]
    public async Task GetById_GbpProvider_ReturnsCurrencyCode()
    {
        dynamic provider = new ExpandoObject();
        provider.id = 7L;
        provider.name = "TravelGate";
        provider.type = "external";
        provider.margin = 8.00m;
        provider.enabled = true;
        provider.currency_iso_code = "GBP";

        SubstituteExtensions.Returns(_factory.MockProviderRepo.GetByIdAsync(7), (dynamic?)provider);

        var response = await _client.GetAsync("/api/providers/7");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        json.RootElement.GetProperty("currency_iso_code").GetString().Should().Be("GBP");
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        // NSubstitute returns null by default for unconfigured args (id=99)
        var response = await _client.GetAsync("/api/providers/99");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
