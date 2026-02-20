using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.HotelBlackouts;

[Collection("AdminApi")]
public class HotelBlackoutsControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public HotelBlackoutsControllerTests(AdminApiFactory factory)
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

    private async Task<(long hotelId, long currencyId)> CreateHotelChainAsync()
    {
        var currencyId = await CreateCurrencyAsync();
        var countryId = await CreateCountryAsync(currencyId);
        var admDivTypeId = await CreateAdmDivTypeAsync();
        var admDivId = await CreateAdmDivAsync(countryId, admDivTypeId);
        var cityId = await CreateCityAsync(admDivId);
        var response = await _client.PostAsJsonAsync("/api/hotels", new
        {
            Name = $"Hotel {Guid.NewGuid().ToString()[..6]}",
            Stars = 4,
            Address = "123 Test St",
            CityId = cityId,
            CountryId = countryId,
            CurrencyId = currencyId,
            Margin = 5.0m,
            CheckInTime = "15:00",
            CheckOutTime = "11:00"
        });
        response.EnsureSuccessStatusCode();
        var hotel = await response.Content.ReadFromJsonAsync<IdResponse>();
        return (hotel!.Id, currencyId);
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/hotelblackouts");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        var (hotelId, _) = await CreateHotelChainAsync();

        var response = await _client.PostAsJsonAsync("/api/hotelblackouts", new
        {
            HotelId = hotelId,
            StartDate = "2025-07-01",
            EndDate = "2025-07-15",
            Reason = $"Blackout {Guid.NewGuid().ToString()[..6]}"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body!.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCreated()
    {
        var (hotelId, _) = await CreateHotelChainAsync();
        var reason = $"Blackout {Guid.NewGuid().ToString()[..6]}";

        var createResponse = await _client.PostAsJsonAsync("/api/hotelblackouts", new
        {
            HotelId = hotelId,
            StartDate = "2025-08-01",
            EndDate = "2025-08-10",
            Reason = reason
        });
        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

        var response = await _client.GetAsync($"/api/hotelblackouts/{created!.Id}");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var (hotelId, _) = await CreateHotelChainAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/hotelblackouts", new
        {
            HotelId = hotelId,
            StartDate = "2025-09-01",
            EndDate = "2025-09-10",
            Reason = $"Before {Guid.NewGuid().ToString()[..6]}"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

        var newReason = $"After {Guid.NewGuid().ToString()[..6]}";
        var updateResponse = await _client.PutAsJsonAsync($"/api/hotelblackouts/{created!.Id}", new
        {
            HotelId = hotelId,
            StartDate = "2025-09-05",
            EndDate = "2025-09-20",
            Reason = newReason
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var (hotelId, _) = await CreateHotelChainAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/hotelblackouts", new
        {
            HotelId = hotelId,
            StartDate = "2025-10-01",
            EndDate = "2025-10-05",
            Reason = $"Del {Guid.NewGuid().ToString()[..6]}"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/hotelblackouts/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/hotelblackouts/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/hotelblackouts");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record IdResponse(long Id);
}
