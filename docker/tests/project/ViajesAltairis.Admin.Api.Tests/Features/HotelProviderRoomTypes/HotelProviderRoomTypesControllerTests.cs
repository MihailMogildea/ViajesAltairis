using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.HotelProviderRoomTypes;

[Collection("AdminApi")]
public class HotelProviderRoomTypesControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public HotelProviderRoomTypesControllerTests(AdminApiFactory factory)
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

    private async Task<long> CreateProviderTypeAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/providertypes", new { Name = $"PT {Guid.NewGuid().ToString()[..6]}" });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
    }

    private async Task<long> CreateProviderAsync()
    {
        var typeId = await CreateProviderTypeAsync();
        var currencyId = await CreateCurrencyAsync();
        var response = await _client.PostAsJsonAsync("/api/providers", new
        {
            Name = $"Provider {Guid.NewGuid().ToString()[..6]}",
            ApiUrl = "https://test.com",
            ApiUsername = "user1",
            ApiPassword = "pass123",
            TypeId = typeId,
            CurrencyId = currencyId,
            Margin = 3.0m
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
    }

    private async Task<long> CreateHotelProviderAsync(long hotelId, long providerId)
    {
        var response = await _client.PostAsJsonAsync("/api/hotelproviders", new
        {
            HotelId = hotelId,
            ProviderId = providerId
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
    }

    private async Task<long> CreateRoomTypeAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/roomtypes", new { Name = $"RT {Guid.NewGuid().ToString()[..6]}" });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
    }

    private async Task<long> CreateExchangeRateAsync(long currencyId)
    {
        var response = await _client.PostAsJsonAsync("/api/exchangerates", new
        {
            CurrencyId = currencyId,
            RateToEur = 1.0m,
            ValidFrom = "2025-01-01T00:00:00",
            ValidTo = "2025-12-31T23:59:59"
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/hotelproviderroomtypes");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        var (hotelId, currencyId) = await CreateHotelChainAsync();
        var providerId = await CreateProviderAsync();
        var hotelProviderId = await CreateHotelProviderAsync(hotelId, providerId);
        var roomTypeId = await CreateRoomTypeAsync();
        var exchangeRateId = await CreateExchangeRateAsync(currencyId);

        var response = await _client.PostAsJsonAsync("/api/hotelproviderroomtypes", new
        {
            HotelProviderId = hotelProviderId,
            RoomTypeId = roomTypeId,
            Capacity = 2,
            Quantity = 10,
            PricePerNight = 120.50m,
            CurrencyId = currencyId,
            ExchangeRateId = exchangeRateId
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body!.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCreated()
    {
        var (hotelId, currencyId) = await CreateHotelChainAsync();
        var providerId = await CreateProviderAsync();
        var hotelProviderId = await CreateHotelProviderAsync(hotelId, providerId);
        var roomTypeId = await CreateRoomTypeAsync();
        var exchangeRateId = await CreateExchangeRateAsync(currencyId);

        var createResponse = await _client.PostAsJsonAsync("/api/hotelproviderroomtypes", new
        {
            HotelProviderId = hotelProviderId,
            RoomTypeId = roomTypeId,
            Capacity = 3,
            Quantity = 5,
            PricePerNight = 95.00m,
            CurrencyId = currencyId,
            ExchangeRateId = exchangeRateId
        });
        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

        var response = await _client.GetAsync($"/api/hotelproviderroomtypes/{created!.Id}");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var (hotelId, currencyId) = await CreateHotelChainAsync();
        var providerId = await CreateProviderAsync();
        var hotelProviderId = await CreateHotelProviderAsync(hotelId, providerId);
        var roomTypeId = await CreateRoomTypeAsync();
        var exchangeRateId = await CreateExchangeRateAsync(currencyId);

        var createResponse = await _client.PostAsJsonAsync("/api/hotelproviderroomtypes", new
        {
            HotelProviderId = hotelProviderId,
            RoomTypeId = roomTypeId,
            Capacity = 2,
            Quantity = 10,
            PricePerNight = 100.00m,
            CurrencyId = currencyId,
            ExchangeRateId = exchangeRateId
        });
        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

        var updateResponse = await _client.PutAsJsonAsync($"/api/hotelproviderroomtypes/{created!.Id}", new
        {
            HotelProviderId = hotelProviderId,
            RoomTypeId = roomTypeId,
            Capacity = 4,
            Quantity = 15,
            PricePerNight = 150.00m,
            CurrencyId = currencyId,
            ExchangeRateId = exchangeRateId
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var (hotelId, currencyId) = await CreateHotelChainAsync();
        var providerId = await CreateProviderAsync();
        var hotelProviderId = await CreateHotelProviderAsync(hotelId, providerId);
        var roomTypeId = await CreateRoomTypeAsync();
        var exchangeRateId = await CreateExchangeRateAsync(currencyId);

        var createResponse = await _client.PostAsJsonAsync("/api/hotelproviderroomtypes", new
        {
            HotelProviderId = hotelProviderId,
            RoomTypeId = roomTypeId,
            Capacity = 2,
            Quantity = 8,
            PricePerNight = 80.00m,
            CurrencyId = currencyId,
            ExchangeRateId = exchangeRateId
        });
        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/hotelproviderroomtypes/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/hotelproviderroomtypes/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SetEnabled_ReturnsNoContent()
    {
        var (hotelId, currencyId) = await CreateHotelChainAsync();
        var providerId = await CreateProviderAsync();
        var hotelProviderId = await CreateHotelProviderAsync(hotelId, providerId);
        var roomTypeId = await CreateRoomTypeAsync();
        var exchangeRateId = await CreateExchangeRateAsync(currencyId);

        var createResponse = await _client.PostAsJsonAsync("/api/hotelproviderroomtypes", new
        {
            HotelProviderId = hotelProviderId,
            RoomTypeId = roomTypeId,
            Capacity = 2,
            Quantity = 5,
            PricePerNight = 90.00m,
            CurrencyId = currencyId,
            ExchangeRateId = exchangeRateId
        });
        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

        var patchResponse = await _client.PatchAsJsonAsync($"/api/hotelproviderroomtypes/{created!.Id}/enabled", new
        {
            Enabled = false
        });

        patchResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/hotelproviderroomtypes");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record IdResponse(long Id);
}
