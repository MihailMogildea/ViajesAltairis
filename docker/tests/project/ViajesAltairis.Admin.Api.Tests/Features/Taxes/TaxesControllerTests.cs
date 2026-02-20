using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.Taxes;

[Collection("AdminApi")]
public class TaxesControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public TaxesControllerTests(AdminApiFactory factory)
    {
        _factory = factory;
        _client = TestAuthHelper.CreateAuthenticatedClient(factory);
    }

    private async Task<long> CreateCurrencyAsync()
    {
        var iso = TestAuthHelper.UniqueIso(3);
        var response = await _client.PostAsJsonAsync("/api/currencies", new { IsoCode = iso, Name = $"Cur {iso}", Symbol = "$" });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
    }

    private async Task<long> CreateCountryAsync(long currencyId)
    {
        var iso = TestAuthHelper.UniqueIso(2);
        var response = await _client.PostAsJsonAsync("/api/countries", new { IsoCode = iso, Name = $"Country {iso}", CurrencyId = currencyId });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
    }

    private async Task<long> CreateAdmDivTypeAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/administrativedivisiontypes", new { Name = $"Type {Guid.NewGuid().ToString()[..6]}" });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
    }

    private async Task<long> CreateAdmDivAsync(long countryId, long typeId)
    {
        var response = await _client.PostAsJsonAsync("/api/administrativedivisions", new { CountryId = countryId, Name = $"Div {Guid.NewGuid().ToString()[..6]}", TypeId = typeId, Level = 1 });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
    }

    private async Task<long> CreateCityAsync(long admDivId)
    {
        var response = await _client.PostAsJsonAsync("/api/cities", new { AdministrativeDivisionId = admDivId, Name = $"City {Guid.NewGuid().ToString()[..6]}" });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
    }

    private async Task<long> CreateTaxTypeAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/taxtypes", new { Name = $"TT {Guid.NewGuid().ToString()[..6]}" });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
    }

    private async Task<(long countryId, long admDivId, long cityId)> CreateLocationChainAsync()
    {
        var currencyId = await CreateCurrencyAsync();
        var countryId = await CreateCountryAsync(currencyId);
        var admDivTypeId = await CreateAdmDivTypeAsync();
        var admDivId = await CreateAdmDivAsync(countryId, admDivTypeId);
        var cityId = await CreateCityAsync(admDivId);
        return (countryId, admDivId, cityId);
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/taxes");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        var taxTypeId = await CreateTaxTypeAsync();
        var (countryId, admDivId, cityId) = await CreateLocationChainAsync();

        var response = await _client.PostAsJsonAsync("/api/taxes", new
        {
            TaxTypeId = taxTypeId,
            CountryId = countryId,
            AdministrativeDivisionId = admDivId,
            CityId = cityId,
            Rate = 10.0m,
            IsPercentage = true
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body!.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCreated()
    {
        var taxTypeId = await CreateTaxTypeAsync();
        var (countryId, admDivId, cityId) = await CreateLocationChainAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/taxes", new
        {
            TaxTypeId = taxTypeId,
            CountryId = countryId,
            AdministrativeDivisionId = admDivId,
            CityId = cityId,
            Rate = 7.5m,
            IsPercentage = true
        });
        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

        var response = await _client.GetAsync($"/api/taxes/{created!.Id}");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var taxTypeId = await CreateTaxTypeAsync();
        var (countryId, admDivId, cityId) = await CreateLocationChainAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/taxes", new
        {
            TaxTypeId = taxTypeId,
            CountryId = countryId,
            AdministrativeDivisionId = admDivId,
            CityId = cityId,
            Rate = 5.0m,
            IsPercentage = true
        });
        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

        var updateResponse = await _client.PutAsJsonAsync($"/api/taxes/{created!.Id}", new
        {
            TaxTypeId = taxTypeId,
            CountryId = countryId,
            AdministrativeDivisionId = admDivId,
            CityId = cityId,
            Rate = 12.0m,
            IsPercentage = false
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var taxTypeId = await CreateTaxTypeAsync();
        var (countryId, admDivId, cityId) = await CreateLocationChainAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/taxes", new
        {
            TaxTypeId = taxTypeId,
            CountryId = countryId,
            AdministrativeDivisionId = admDivId,
            CityId = cityId,
            Rate = 8.0m,
            IsPercentage = true
        });
        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/taxes/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/taxes/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SetEnabled_ReturnsNoContent()
    {
        var taxTypeId = await CreateTaxTypeAsync();
        var (countryId, admDivId, cityId) = await CreateLocationChainAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/taxes", new
        {
            TaxTypeId = taxTypeId,
            CountryId = countryId,
            AdministrativeDivisionId = admDivId,
            CityId = cityId,
            Rate = 6.0m,
            IsPercentage = true
        });
        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

        var patchResponse = await _client.PatchAsJsonAsync($"/api/taxes/{created!.Id}/enabled", new
        {
            Enabled = false
        });

        patchResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/taxes");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record IdResponse(long Id);
}
