using System.Dynamic;
using System.Net;
using System.Text.Json;
using NSubstitute;
using ViajesAltairis.Providers.Api.Tests.Fixtures;

namespace ViajesAltairis.Providers.Api.Tests.Tests;

public class SyncEndpointTests : IClassFixture<ProvidersApiFactory>
{
    private readonly HttpClient _client;
    private readonly ProvidersApiFactory _factory;

    public SyncEndpointTests(ProvidersApiFactory factory)
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
    public async Task Sync_ExternalProvider_Returns202()
    {
        SubstituteExtensions.Returns(_factory.MockProviderRepo.GetByIdAsync(6), (dynamic?)MakeExternalProvider());
        _factory.MockProviderRepo.TrySetSyncStatusAsync(6, "updating", null).Returns(true);

        var response = await _client.PostAsync("/api/providers/6/sync", null);

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        json.RootElement.GetProperty("message").GetString().Should().Contain("Sync started");
    }

    [Fact]
    public async Task Sync_InternalProvider_Returns400()
    {
        SubstituteExtensions.Returns(_factory.MockProviderRepo.GetByIdAsync(1), (dynamic?)MakeInternalProvider());

        var response = await _client.PostAsync("/api/providers/1/sync", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Sync_NotFound_Returns404()
    {
        // NSubstitute returns null by default for unconfigured args (id=99)
        var response = await _client.PostAsync("/api/providers/99/sync", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Sync_AlreadySyncing_Returns409()
    {
        SubstituteExtensions.Returns(_factory.MockProviderRepo.GetByIdAsync(6), (dynamic?)MakeExternalProvider());
        _factory.MockProviderRepo.TrySetSyncStatusAsync(6, "updating", null).Returns(false);

        var response = await _client.PostAsync("/api/providers/6/sync", null);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
