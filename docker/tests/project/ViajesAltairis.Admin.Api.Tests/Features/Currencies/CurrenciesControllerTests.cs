using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.Currencies;

[Collection("AdminApi")]
public class CurrenciesControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public CurrenciesControllerTests(AdminApiFactory factory)
    {
        _factory = factory;
        _client = TestAuthHelper.CreateAuthenticatedClient(factory);
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/currencies");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        var isoCode = TestAuthHelper.UniqueIso(3);
        var response = await _client.PostAsJsonAsync("/api/currencies", new
        {
            IsoCode = isoCode,
            Name = $"Test Currency {isoCode}",
            Symbol = "$"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<CurrencyResponse>();
        body!.Id.Should().BeGreaterThan(0);
        body.IsoCode.Should().Be(isoCode);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCreated()
    {
        var isoCode = TestAuthHelper.UniqueIso(3);
        var createResponse = await _client.PostAsJsonAsync("/api/currencies", new
        {
            IsoCode = isoCode,
            Name = $"Get Test {isoCode}",
            Symbol = "G"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<CurrencyResponse>();

        var response = await _client.GetAsync($"/api/currencies/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<CurrencyResponse>();
        body!.Id.Should().Be(created.Id);
        body.IsoCode.Should().Be(isoCode);
    }

    [Fact]
    public async Task GetById_NonExistent_Returns404()
    {
        var response = await _client.GetAsync("/api/currencies/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var isoCode = TestAuthHelper.UniqueIso(3);
        var createResponse = await _client.PostAsJsonAsync("/api/currencies", new
        {
            IsoCode = isoCode,
            Name = "Before Update",
            Symbol = "B"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<CurrencyResponse>();

        var newIso = TestAuthHelper.UniqueIso(3);
        var updateResponse = await _client.PutAsJsonAsync($"/api/currencies/{created!.Id}", new
        {
            IsoCode = newIso,
            Name = "After Update",
            Symbol = "A"
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await updateResponse.Content.ReadFromJsonAsync<CurrencyResponse>();
        body!.Name.Should().Be("After Update");
        body.IsoCode.Should().Be(newIso);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var isoCode = TestAuthHelper.UniqueIso(3);
        var createResponse = await _client.PostAsJsonAsync("/api/currencies", new
        {
            IsoCode = isoCode,
            Name = "Delete Test",
            Symbol = "D"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<CurrencyResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/currencies/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify it's gone
        var getResponse = await _client.GetAsync($"/api/currencies/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WithEmptyIsoCode_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/currencies", new
        {
            IsoCode = "",
            Name = "Invalid",
            Symbol = "$"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/currencies");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record CurrencyResponse(long Id, string IsoCode, string Name, string Symbol, DateTime CreatedAt);
}
