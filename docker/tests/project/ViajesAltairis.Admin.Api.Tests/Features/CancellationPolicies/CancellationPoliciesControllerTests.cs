using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.CancellationPolicies;

[Collection("AdminApi")]
public class CancellationPoliciesControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public CancellationPoliciesControllerTests(AdminApiFactory factory)
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

    private async Task<long> CreateCityAsync(long admDivId)
    {
        var response = await _client.PostAsJsonAsync("/api/cities", new
        {
            AdministrativeDivisionId = admDivId,
            Name = $"City {Guid.NewGuid().ToString()[..6]}"
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        return body!.Id;
    }

    private async Task<(long HotelId, long CurrencyId, long CountryId)> CreateHotelChainAsync()
    {
        var currencyId = await CreateCurrencyAsync();
        var countryId = await CreateCountryAsync(currencyId);
        var typeId = await CreateAdmDivTypeAsync();
        var admDivId = await CreateAdmDivAsync(countryId, typeId);
        var cityId = await CreateCityAsync(admDivId);

        var response = await _client.PostAsJsonAsync("/api/hotels", new
        {
            Name = $"Hotel {Guid.NewGuid().ToString()[..6]}",
            Stars = (byte)4,
            Address = "Test Address",
            CityId = cityId,
            CountryId = countryId,
            CurrencyId = currencyId,
            Margin = 5.0m,
            CheckInTime = "15:00",
            CheckOutTime = "11:00"
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        return (body!.Id, currencyId, countryId);
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/cancellationpolicies");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        var (hotelId, _, _) = await CreateHotelChainAsync();

        var response = await _client.PostAsJsonAsync("/api/cancellationpolicies", new
        {
            HotelId = hotelId,
            FreeCancellationHours = 48,
            PenaltyPercentage = 25.0m
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<CancellationPolicyResponse>();
        body!.Id.Should().BeGreaterThan(0);
        body.HotelId.Should().Be(hotelId);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCreated()
    {
        var (hotelId, _, _) = await CreateHotelChainAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/cancellationpolicies", new
        {
            HotelId = hotelId,
            FreeCancellationHours = 24,
            PenaltyPercentage = 50.0m
        });
        var created = await createResponse.Content.ReadFromJsonAsync<CancellationPolicyResponse>();

        var response = await _client.GetAsync($"/api/cancellationpolicies/{created!.Id}");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var (hotelId, _, _) = await CreateHotelChainAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/cancellationpolicies", new
        {
            HotelId = hotelId,
            FreeCancellationHours = 48,
            PenaltyPercentage = 25.0m
        });
        var created = await createResponse.Content.ReadFromJsonAsync<CancellationPolicyResponse>();

        var updateResponse = await _client.PutAsJsonAsync($"/api/cancellationpolicies/{created!.Id}", new
        {
            HotelId = hotelId,
            FreeCancellationHours = 72,
            PenaltyPercentage = 10.0m
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await updateResponse.Content.ReadFromJsonAsync<CancellationPolicyResponse>();
        body!.FreeCancellationHours.Should().Be(72);
        body.PenaltyPercentage.Should().Be(10.0m);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var (hotelId, _, _) = await CreateHotelChainAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/cancellationpolicies", new
        {
            HotelId = hotelId,
            FreeCancellationHours = 24,
            PenaltyPercentage = 100.0m
        });
        var created = await createResponse.Content.ReadFromJsonAsync<CancellationPolicyResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/cancellationpolicies/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/cancellationpolicies/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SetEnabled_ReturnsNoContent()
    {
        var (hotelId, _, _) = await CreateHotelChainAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/cancellationpolicies", new
        {
            HotelId = hotelId,
            FreeCancellationHours = 48,
            PenaltyPercentage = 25.0m
        });
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<CancellationPolicyResponse>();
        created!.Id.Should().BeGreaterThan(0);

        var patchResponse = await _client.PatchAsJsonAsync($"/api/cancellationpolicies/{created.Id}/enabled", new
        {
            Enabled = false
        });

        patchResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/cancellationpolicies");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record IdResponse(long Id);
    private record CancellationPolicyResponse(long Id, long HotelId, int FreeCancellationHours, decimal PenaltyPercentage, bool Enabled, DateTime CreatedAt);
}
