using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.Countries;

[Collection("AdminApi")]
public class CountriesControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public CountriesControllerTests(AdminApiFactory factory)
    {
        _factory = factory;
        _client = TestAuthHelper.CreateAuthenticatedClient(factory);
    }

    private async Task<long> CreateCurrencyAsync()
    {
        var iso = TestAuthHelper.UniqueIso(3);
        var response = await _client.PostAsJsonAsync("/api/currencies", new
        {
            IsoCode = iso,
            Name = $"Cur {iso}",
            Symbol = "$"
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        return body!.Id;
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/countries");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        var currencyId = await CreateCurrencyAsync();
        var iso = TestAuthHelper.UniqueIso(2);
        var response = await _client.PostAsJsonAsync("/api/countries", new
        {
            IsoCode = iso,
            Name = $"Country {iso}",
            CurrencyId = currencyId
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<CountryResponse>();
        body!.Id.Should().BeGreaterThan(0);
        body.IsoCode.Should().Be(iso);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCreated()
    {
        var currencyId = await CreateCurrencyAsync();
        var iso = TestAuthHelper.UniqueIso(2);
        var createResponse = await _client.PostAsJsonAsync("/api/countries", new
        {
            IsoCode = iso,
            Name = $"Country {iso}",
            CurrencyId = currencyId
        });
        var created = await createResponse.Content.ReadFromJsonAsync<CountryResponse>();

        var response = await _client.GetAsync($"/api/countries/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<CountryResponse>();
        body!.Id.Should().Be(created.Id);
        body.IsoCode.Should().Be(iso);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var currencyId = await CreateCurrencyAsync();
        var iso = TestAuthHelper.UniqueIso(2);
        var createResponse = await _client.PostAsJsonAsync("/api/countries", new
        {
            IsoCode = iso,
            Name = "Before Update",
            CurrencyId = currencyId
        });
        var created = await createResponse.Content.ReadFromJsonAsync<CountryResponse>();

        var newIso = TestAuthHelper.UniqueIso(2);
        var updateResponse = await _client.PutAsJsonAsync($"/api/countries/{created!.Id}", new
        {
            IsoCode = newIso,
            Name = "After Update",
            CurrencyId = currencyId
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await updateResponse.Content.ReadFromJsonAsync<CountryResponse>();
        body!.Name.Should().Be("After Update");
        body.IsoCode.Should().Be(newIso);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var currencyId = await CreateCurrencyAsync();
        var iso = TestAuthHelper.UniqueIso(2);
        var createResponse = await _client.PostAsJsonAsync("/api/countries", new
        {
            IsoCode = iso,
            Name = "Delete Test",
            CurrencyId = currencyId
        });
        var created = await createResponse.Content.ReadFromJsonAsync<CountryResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/countries/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/countries/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SetEnabled_ReturnsNoContent()
    {
        var currencyId = await CreateCurrencyAsync();
        var iso = TestAuthHelper.UniqueIso(2);
        var createResponse = await _client.PostAsJsonAsync("/api/countries", new
        {
            IsoCode = iso,
            Name = $"Enabled Test {iso}",
            CurrencyId = currencyId
        });
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<CountryResponse>();
        created!.Id.Should().BeGreaterThan(0);

        var patchResponse = await _client.PatchAsJsonAsync($"/api/countries/{created.Id}/enabled", new
        {
            Enabled = false
        });

        patchResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/countries");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record IdResponse(long Id);
    private record CountryResponse(long Id, string IsoCode, string Name, long CurrencyId, bool Enabled, DateTime CreatedAt);
}
