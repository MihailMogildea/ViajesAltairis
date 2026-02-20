using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.HotelProviderRoomTypeAmenities;

[Collection("AdminApi")]
public class HotelProviderRoomTypeAmenitiesControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public HotelProviderRoomTypeAmenitiesControllerTests(AdminApiFactory factory)
    {
        _factory = factory;
        _client = TestAuthHelper.CreateAuthenticatedClient(factory);
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/hotelproviderroomtypeamenities");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Assign_Returns201()
    {
        var hprtId = await CreateFullChainAsync();
        var categoryId = await CreateAmenityCategoryAsync();
        var amenityId = await CreateAmenityAsync(categoryId);

        var response = await _client.PostAsJsonAsync("/api/hotelproviderroomtypeamenities", new
        {
            HotelProviderRoomTypeId = hprtId,
            AmenityId = amenityId
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body!.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Assign_ThenDelete_Returns204()
    {
        var hprtId = await CreateFullChainAsync();
        var categoryId = await CreateAmenityCategoryAsync();
        var amenityId = await CreateAmenityAsync(categoryId);

        var assignResponse = await _client.PostAsJsonAsync("/api/hotelproviderroomtypeamenities", new
        {
            HotelProviderRoomTypeId = hprtId,
            AmenityId = amenityId
        });
        var created = await assignResponse.Content.ReadFromJsonAsync<IdResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/hotelproviderroomtypeamenities/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/hotelproviderroomtypeamenities");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// Builds the full FK chain: currency -> country -> admDivType -> admDiv -> city -> hotel,
    /// providerType -> provider -> hotelProvider, roomType, exchangeRate -> hotelProviderRoomType.
    /// </summary>
    private async Task<long> CreateFullChainAsync()
    {
        var (hotelId, currencyId) = await CreateHotelChainAsync();
        var providerTypeId = await CreateProviderTypeAsync();
        var providerId = await CreateProviderAsync(providerTypeId);
        var hotelProviderId = await CreateHotelProviderAsync(hotelId, providerId);
        var roomTypeId = await CreateRoomTypeAsync();
        var exchangeRateId = await CreateExchangeRateAsync(currencyId);
        return await CreateHotelProviderRoomTypeAsync(hotelProviderId, roomTypeId, currencyId, exchangeRateId);
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

    private async Task<long> CreateProviderTypeAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/providertypes", new { Name = $"PT {Guid.NewGuid().ToString()[..6]}" });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
    }

    private async Task<long> CreateProviderAsync(long providerTypeId)
    {
        var currencyId = await CreateCurrencyAsync();
        var response = await _client.PostAsJsonAsync("/api/providers", new
        {
            TypeId = providerTypeId, CurrencyId = currencyId,
            Name = $"Provider {Guid.NewGuid().ToString()[..6]}", Margin = 3.0m
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
    }

    private async Task<long> CreateHotelProviderAsync(long hotelId, long providerId)
    {
        var response = await _client.PostAsJsonAsync("/api/hotelproviders", new { HotelId = hotelId, ProviderId = providerId });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
    }

    private async Task<long> CreateRoomTypeAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/roomtypes", new { Name = $"Room {Guid.NewGuid().ToString()[..6]}" });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
    }

    private async Task<long> CreateExchangeRateAsync(long currencyId)
    {
        var response = await _client.PostAsJsonAsync("/api/exchangerates", new
        {
            CurrencyId = currencyId, RateToEur = 1.15m,
            ValidFrom = "2025-01-01T00:00:00", ValidTo = "2025-12-31T23:59:59"
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
    }

    private async Task<long> CreateHotelProviderRoomTypeAsync(long hotelProviderId, long roomTypeId, long currencyId, long exchangeRateId)
    {
        var response = await _client.PostAsJsonAsync("/api/hotelproviderroomtypes", new
        {
            HotelProviderId = hotelProviderId, RoomTypeId = roomTypeId,
            Capacity = 2, Quantity = 10, PricePerNight = 99.99m,
            CurrencyId = currencyId, ExchangeRateId = exchangeRateId
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
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
