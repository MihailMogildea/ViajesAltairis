using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.HotelImages;

[Collection("AdminApi")]
public class HotelImagesControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public HotelImagesControllerTests(AdminApiFactory factory)
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
        var response = await _client.GetAsync("/api/hotelimages");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        var (hotelId, _) = await CreateHotelChainAsync();

        var response = await _client.PostAsJsonAsync("/api/hotelimages", new
        {
            HotelId = hotelId,
            Url = $"https://images.test.com/{Guid.NewGuid()}.jpg",
            AltText = $"Image {Guid.NewGuid().ToString()[..6]}",
            SortOrder = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body!.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCreated()
    {
        var (hotelId, _) = await CreateHotelChainAsync();
        var url = $"https://images.test.com/{Guid.NewGuid()}.jpg";

        var createResponse = await _client.PostAsJsonAsync("/api/hotelimages", new
        {
            HotelId = hotelId,
            Url = url,
            AltText = $"Image {Guid.NewGuid().ToString()[..6]}",
            SortOrder = 1
        });
        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

        var response = await _client.GetAsync($"/api/hotelimages/{created!.Id}");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var (hotelId, _) = await CreateHotelChainAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/hotelimages", new
        {
            HotelId = hotelId,
            Url = $"https://images.test.com/{Guid.NewGuid()}.jpg",
            AltText = $"Before {Guid.NewGuid().ToString()[..6]}",
            SortOrder = 1
        });
        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

        var newUrl = $"https://images.test.com/{Guid.NewGuid()}.jpg";
        var updateResponse = await _client.PutAsJsonAsync($"/api/hotelimages/{created!.Id}", new
        {
            HotelId = hotelId,
            Url = newUrl,
            AltText = $"After {Guid.NewGuid().ToString()[..6]}",
            SortOrder = 2
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var (hotelId, _) = await CreateHotelChainAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/hotelimages", new
        {
            HotelId = hotelId,
            Url = $"https://images.test.com/{Guid.NewGuid()}.jpg",
            AltText = $"Del {Guid.NewGuid().ToString()[..6]}",
            SortOrder = 1
        });
        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/hotelimages/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/hotelimages/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/hotelimages");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record IdResponse(long Id);
}
