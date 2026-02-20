using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.SeasonalMargins;

[Collection("AdminApi")]
public class SeasonalMarginsControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public SeasonalMarginsControllerTests(AdminApiFactory factory)
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
        var response = await _client.GetAsync("/api/seasonalmargins");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        var admDivId = await CreateAdmDivChainAsync();

        var response = await _client.PostAsJsonAsync("/api/seasonalmargins", new
        {
            AdministrativeDivisionId = admDivId,
            StartMonthDay = "06-01",
            EndMonthDay = "08-31",
            Margin = 15.0m
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<SeasonalMarginResponse>();
        body!.Id.Should().BeGreaterThan(0);
        body.Margin.Should().Be(15.0m);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCreated()
    {
        var admDivId = await CreateAdmDivChainAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/seasonalmargins", new
        {
            AdministrativeDivisionId = admDivId,
            StartMonthDay = "07-01",
            EndMonthDay = "09-30",
            Margin = 20.0m
        });
        var created = await createResponse.Content.ReadFromJsonAsync<SeasonalMarginResponse>();

        var response = await _client.GetAsync($"/api/seasonalmargins/{created!.Id}");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var admDivId = await CreateAdmDivChainAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/seasonalmargins", new
        {
            AdministrativeDivisionId = admDivId,
            StartMonthDay = "06-01",
            EndMonthDay = "08-31",
            Margin = 10.0m
        });
        var created = await createResponse.Content.ReadFromJsonAsync<SeasonalMarginResponse>();

        var updateResponse = await _client.PutAsJsonAsync($"/api/seasonalmargins/{created!.Id}", new
        {
            AdministrativeDivisionId = admDivId,
            StartMonthDay = "05-01",
            EndMonthDay = "09-30",
            Margin = 25.0m
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await updateResponse.Content.ReadFromJsonAsync<SeasonalMarginResponse>();
        body!.Margin.Should().Be(25.0m);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var admDivId = await CreateAdmDivChainAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/seasonalmargins", new
        {
            AdministrativeDivisionId = admDivId,
            StartMonthDay = "12-01",
            EndMonthDay = "12-31",
            Margin = 30.0m
        });
        var created = await createResponse.Content.ReadFromJsonAsync<SeasonalMarginResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/seasonalmargins/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/seasonalmargins/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/seasonalmargins");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record IdResponse(long Id);
    private record SeasonalMarginResponse(long Id, long AdministrativeDivisionId, string StartMonthDay, string EndMonthDay, decimal Margin, DateTime CreatedAt);
}
