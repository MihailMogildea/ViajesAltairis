using System.Dynamic;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using NSubstitute;
using ViajesAltairis.ProvidersApi.ExternalClients;
using ViajesAltairis.Providers.Api.Tests.Fixtures;

namespace ViajesAltairis.Providers.Api.Tests.Tests;

public class ExternalOperationsTests : IClassFixture<ProvidersApiFactory>
{
    private readonly HttpClient _client;
    private readonly ProvidersApiFactory _factory;

    public ExternalOperationsTests(ProvidersApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private static dynamic MakeExternalProvider(long id = 6, string name = "HotelBeds")
    {
        dynamic p = new ExpandoObject();
        p.id = id;
        p.name = name;
        p.type = "external";
        p.margin = 10.00m;
        p.enabled = true;
        p.currency_iso_code = "EUR";
        return p;
    }

    private static dynamic MakeInternalProvider(long id = 1)
    {
        dynamic p = new ExpandoObject();
        p.id = id;
        p.name = "ViajesAltairis";
        p.type = "internal";
        p.margin = 0.00m;
        p.enabled = true;
        p.currency_iso_code = "EUR";
        return p;
    }

    [Fact]
    public async Task Availability_ExternalProvider_ReturnsResults()
    {
        SubstituteExtensions.Returns(_factory.MockProviderRepo.GetByIdAsync(6), (dynamic?)MakeExternalProvider());

        var response = await _client.GetAsync(
            "/api/providers/6/availability?city=Palma&checkIn=2026-06-01&checkOut=2026-06-05&guests=2");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        json.RootElement.GetProperty("hotels").ValueKind.Should().Be(JsonValueKind.Array);
    }

    [Fact]
    public async Task Availability_InternalProvider_Returns400()
    {
        SubstituteExtensions.Returns(_factory.MockProviderRepo.GetByIdAsync(1), (dynamic?)MakeInternalProvider());

        var response = await _client.GetAsync(
            "/api/providers/1/availability?city=Palma&checkIn=2026-06-01&checkOut=2026-06-05&guests=2");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Availability_NotFound_Returns404()
    {
        // NSubstitute returns null by default for unconfigured args (id=99)
        var response = await _client.GetAsync(
            "/api/providers/99/availability?city=Palma&checkIn=2026-06-01&checkOut=2026-06-05&guests=2");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Book_ExternalProvider_ReturnsReference()
    {
        SubstituteExtensions.Returns(_factory.MockProviderRepo.GetByIdAsync(6), (dynamic?)MakeExternalProvider());

        var request = new BookingRequest("Hotel Altairis", "double", "half_board",
            new DateTime(2026, 6, 1), new DateTime(2026, 6, 5), 2, "John Doe", "john@test.com");

        var response = await _client.PostAsJsonAsync("/api/providers/6/book", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        json.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();
        json.RootElement.GetProperty("bookingReference").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Cancel_ExternalProvider_ReturnsSuccess()
    {
        SubstituteExtensions.Returns(_factory.MockProviderRepo.GetByIdAsync(6), (dynamic?)MakeExternalProvider());

        var response = await _client.DeleteAsync("/api/providers/6/bookings/HB-ABC12345");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        json.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task Cancel_InternalProvider_Returns400()
    {
        SubstituteExtensions.Returns(_factory.MockProviderRepo.GetByIdAsync(1), (dynamic?)MakeInternalProvider());

        var response = await _client.DeleteAsync("/api/providers/1/bookings/REF123");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
