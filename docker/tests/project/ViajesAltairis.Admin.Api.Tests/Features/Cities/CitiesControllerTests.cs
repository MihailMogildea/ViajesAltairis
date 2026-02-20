using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.Cities;

[Collection("AdminApi")]
public class CitiesControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public CitiesControllerTests(AdminApiFactory factory)
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

    private async Task<long> CreateCountryAsync(long currencyId)
    {
        var iso = TestAuthHelper.UniqueIso(2);
        var response = await _client.PostAsJsonAsync("/api/countries", new
        {
            IsoCode = iso,
            Name = $"Country {iso}",
            CurrencyId = currencyId
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        return body!.Id;
    }

    private async Task<long> CreateAdmDivTypeAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/administrativedivisiontypes", new
        {
            Name = $"ADT {Guid.NewGuid().ToString()[..6]}"
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        return body!.Id;
    }

    private async Task<long> CreateAdmDivAsync(long countryId, long typeId)
    {
        var response = await _client.PostAsJsonAsync("/api/administrativedivisions", new
        {
            CountryId = countryId,
            Name = $"AD {Guid.NewGuid().ToString()[..6]}",
            TypeId = typeId,
            Level = (byte)1
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        return body!.Id;
    }

    private async Task<long> CreateAdmDivChainAsync()
    {
        var currencyId = await CreateCurrencyAsync();
        var countryId = await CreateCountryAsync(currencyId);
        var typeId = await CreateAdmDivTypeAsync();
        return await CreateAdmDivAsync(countryId, typeId);
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/cities");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        var admDivId = await CreateAdmDivChainAsync();
        var name = $"City {Guid.NewGuid().ToString()[..6]}";

        var response = await _client.PostAsJsonAsync("/api/cities", new
        {
            AdministrativeDivisionId = admDivId,
            Name = name
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<CityResponse>();
        body!.Id.Should().BeGreaterThan(0);
        body.Name.Should().Be(name);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCreated()
    {
        var admDivId = await CreateAdmDivChainAsync();
        var name = $"City {Guid.NewGuid().ToString()[..6]}";

        var createResponse = await _client.PostAsJsonAsync("/api/cities", new
        {
            AdministrativeDivisionId = admDivId,
            Name = name
        });
        var created = await createResponse.Content.ReadFromJsonAsync<CityResponse>();

        var response = await _client.GetAsync($"/api/cities/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<CityResponse>();
        body!.Id.Should().Be(created.Id);
        body.Name.Should().Be(name);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var admDivId = await CreateAdmDivChainAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/cities", new
        {
            AdministrativeDivisionId = admDivId,
            Name = "Before Update"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<CityResponse>();

        var newName = $"After {Guid.NewGuid().ToString()[..6]}";
        var updateResponse = await _client.PutAsJsonAsync($"/api/cities/{created!.Id}", new
        {
            AdministrativeDivisionId = admDivId,
            Name = newName
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await updateResponse.Content.ReadFromJsonAsync<CityResponse>();
        body!.Name.Should().Be(newName);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var admDivId = await CreateAdmDivChainAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/cities", new
        {
            AdministrativeDivisionId = admDivId,
            Name = $"Del {Guid.NewGuid().ToString()[..6]}"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<CityResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/cities/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/cities/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SetEnabled_ReturnsNoContent()
    {
        var admDivId = await CreateAdmDivChainAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/cities", new
        {
            AdministrativeDivisionId = admDivId,
            Name = $"Enabled {Guid.NewGuid().ToString()[..6]}"
        });
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<CityResponse>();
        created!.Id.Should().BeGreaterThan(0);

        var patchResponse = await _client.PatchAsJsonAsync($"/api/cities/{created.Id}/enabled", new
        {
            Enabled = false
        });

        patchResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/cities");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record IdResponse(long Id);
    private record CityResponse(long Id, long AdministrativeDivisionId, string Name, bool Enabled, DateTime CreatedAt);
}
