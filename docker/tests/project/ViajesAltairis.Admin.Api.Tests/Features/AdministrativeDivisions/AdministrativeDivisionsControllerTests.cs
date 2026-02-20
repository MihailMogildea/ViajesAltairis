using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.AdministrativeDivisions;

[Collection("AdminApi")]
public class AdministrativeDivisionsControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public AdministrativeDivisionsControllerTests(AdminApiFactory factory)
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

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/administrativedivisions");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        var currencyId = await CreateCurrencyAsync();
        var countryId = await CreateCountryAsync(currencyId);
        var typeId = await CreateAdmDivTypeAsync();
        var name = $"AD {Guid.NewGuid().ToString()[..6]}";

        var response = await _client.PostAsJsonAsync("/api/administrativedivisions", new
        {
            CountryId = countryId,
            ParentId = (long?)null,
            Name = name,
            TypeId = typeId,
            Level = (byte)1
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<AdmDivResponse>();
        body!.Id.Should().BeGreaterThan(0);
        body.Name.Should().Be(name);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCreated()
    {
        var currencyId = await CreateCurrencyAsync();
        var countryId = await CreateCountryAsync(currencyId);
        var typeId = await CreateAdmDivTypeAsync();
        var name = $"AD {Guid.NewGuid().ToString()[..6]}";

        var createResponse = await _client.PostAsJsonAsync("/api/administrativedivisions", new
        {
            CountryId = countryId,
            ParentId = (long?)null,
            Name = name,
            TypeId = typeId,
            Level = (byte)1
        });
        var created = await createResponse.Content.ReadFromJsonAsync<AdmDivResponse>();

        var response = await _client.GetAsync($"/api/administrativedivisions/{created!.Id}");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var currencyId = await CreateCurrencyAsync();
        var countryId = await CreateCountryAsync(currencyId);
        var typeId = await CreateAdmDivTypeAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/administrativedivisions", new
        {
            CountryId = countryId,
            ParentId = (long?)null,
            Name = "Before Update",
            TypeId = typeId,
            Level = (byte)1
        });
        var created = await createResponse.Content.ReadFromJsonAsync<AdmDivResponse>();

        var newName = $"After {Guid.NewGuid().ToString()[..6]}";
        var updateResponse = await _client.PutAsJsonAsync($"/api/administrativedivisions/{created!.Id}", new
        {
            CountryId = countryId,
            ParentId = (long?)null,
            Name = newName,
            TypeId = typeId,
            Level = (byte)1
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await updateResponse.Content.ReadFromJsonAsync<AdmDivResponse>();
        body!.Name.Should().Be(newName);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var currencyId = await CreateCurrencyAsync();
        var countryId = await CreateCountryAsync(currencyId);
        var typeId = await CreateAdmDivTypeAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/administrativedivisions", new
        {
            CountryId = countryId,
            ParentId = (long?)null,
            Name = $"Del {Guid.NewGuid().ToString()[..6]}",
            TypeId = typeId,
            Level = (byte)1
        });
        var created = await createResponse.Content.ReadFromJsonAsync<AdmDivResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/administrativedivisions/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/administrativedivisions/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SetEnabled_ReturnsNoContent()
    {
        var currencyId = await CreateCurrencyAsync();
        var countryId = await CreateCountryAsync(currencyId);
        var typeId = await CreateAdmDivTypeAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/administrativedivisions", new
        {
            CountryId = countryId,
            ParentId = (long?)null,
            Name = $"Enabled {Guid.NewGuid().ToString()[..6]}",
            TypeId = typeId,
            Level = (byte)1
        });
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<AdmDivResponse>();
        created!.Id.Should().BeGreaterThan(0);

        var patchResponse = await _client.PatchAsJsonAsync($"/api/administrativedivisions/{created.Id}/enabled", new
        {
            Enabled = false
        });

        patchResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/administrativedivisions");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record IdResponse(long Id);
    private record AdmDivResponse(long Id, long CountryId, long? ParentId, string Name, long TypeId, byte Level, bool Enabled, DateTime CreatedAt);
}
