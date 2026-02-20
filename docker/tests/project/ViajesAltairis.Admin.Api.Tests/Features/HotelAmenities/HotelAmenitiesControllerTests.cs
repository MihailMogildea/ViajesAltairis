using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.HotelAmenities;

[Collection("AdminApi")]
public class HotelAmenitiesControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public HotelAmenitiesControllerTests(AdminApiFactory factory)
    {
        _factory = factory;
        _client = TestAuthHelper.CreateAuthenticatedClient(factory);
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/hotelamenities");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Assign_Returns201()
    {
        var (hotelId, _) = await CreateHotelChainAsync();
        var categoryId = await CreateAmenityCategoryAsync();
        var amenityId = await CreateAmenityAsync(categoryId);

        var response = await _client.PostAsJsonAsync("/api/hotelamenities", new
        {
            HotelId = hotelId,
            AmenityId = amenityId
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body!.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Assign_ThenDelete_Returns204()
    {
        var (hotelId, _) = await CreateHotelChainAsync();
        var categoryId = await CreateAmenityCategoryAsync();
        var amenityId = await CreateAmenityAsync(categoryId);

        var assignResponse = await _client.PostAsJsonAsync("/api/hotelamenities", new
        {
            HotelId = hotelId,
            AmenityId = amenityId
        });
        var created = await assignResponse.Content.ReadFromJsonAsync<IdResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/hotelamenities/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/hotelamenities");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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
        var response = await _client.PostAsJsonAsync("/api/administrativedivisiontypes", new { Name = $"ADT {Guid.NewGuid().ToString()[..6]}" });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
    }

    private async Task<long> CreateAdmDivAsync(long countryId, long typeId)
    {
        var response = await _client.PostAsJsonAsync("/api/administrativedivisions", new
        {
            CountryId = countryId, ParentId = (long?)null, Name = $"AD {Guid.NewGuid().ToString()[..6]}", TypeId = typeId, Level = (byte)1
        });
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
            Stars = 4, Address = "123 Test St", CityId = cityId,
            CountryId = countryId, CurrencyId = currencyId,
            Margin = 5.0m, CheckInTime = "15:00", CheckOutTime = "11:00"
        });
        response.EnsureSuccessStatusCode();
        var hotel = await response.Content.ReadFromJsonAsync<IdResponse>();
        return (hotel!.Id, currencyId);
    }

    private async Task<long> CreateAmenityCategoryAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/amenitycategories", new { Name = $"Cat {Guid.NewGuid().ToString()[..6]}" });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
    }

    private async Task<long> CreateAmenityAsync(long categoryId)
    {
        var response = await _client.PostAsJsonAsync("/api/amenities", new { CategoryId = categoryId, Name = $"Amenity {Guid.NewGuid().ToString()[..6]}" });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
    }

    private record IdResponse(long Id);
}
