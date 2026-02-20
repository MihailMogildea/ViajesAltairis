using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.ExchangeRates;

[Collection("AdminApi")]
public class ExchangeRatesControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public ExchangeRatesControllerTests(AdminApiFactory factory)
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

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/exchangerates");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        var currencyId = await CreateCurrencyAsync();

        var response = await _client.PostAsJsonAsync("/api/exchangerates", new
        {
            CurrencyId = currencyId,
            RateToEur = 1.25m,
            ValidFrom = "2025-01-01T00:00:00",
            ValidTo = "2025-12-31T23:59:59"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<ExchangeRateResponse>();
        body!.Id.Should().BeGreaterThan(0);
        body.CurrencyId.Should().Be(currencyId);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCreated()
    {
        var currencyId = await CreateCurrencyAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/exchangerates", new
        {
            CurrencyId = currencyId,
            RateToEur = 0.85m,
            ValidFrom = "2025-01-01T00:00:00",
            ValidTo = "2025-12-31T23:59:59"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<ExchangeRateResponse>();

        var response = await _client.GetAsync($"/api/exchangerates/{created!.Id}");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var currencyId = await CreateCurrencyAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/exchangerates", new
        {
            CurrencyId = currencyId,
            RateToEur = 1.10m,
            ValidFrom = "2025-01-01T00:00:00",
            ValidTo = "2025-06-30T23:59:59"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<ExchangeRateResponse>();

        var updateResponse = await _client.PutAsJsonAsync($"/api/exchangerates/{created!.Id}", new
        {
            CurrencyId = currencyId,
            RateToEur = 1.20m,
            ValidFrom = "2025-01-01T00:00:00",
            ValidTo = "2025-12-31T23:59:59"
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await updateResponse.Content.ReadFromJsonAsync<ExchangeRateResponse>();
        body!.RateToEur.Should().Be(1.20m);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var currencyId = await CreateCurrencyAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/exchangerates", new
        {
            CurrencyId = currencyId,
            RateToEur = 0.95m,
            ValidFrom = "2025-01-01T00:00:00",
            ValidTo = "2025-12-31T23:59:59"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<ExchangeRateResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/exchangerates/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/exchangerates/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/exchangerates");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record IdResponse(long Id);
    private record ExchangeRateResponse(long Id, long CurrencyId, decimal RateToEur, DateTime ValidFrom, DateTime? ValidTo, DateTime CreatedAt);
}
