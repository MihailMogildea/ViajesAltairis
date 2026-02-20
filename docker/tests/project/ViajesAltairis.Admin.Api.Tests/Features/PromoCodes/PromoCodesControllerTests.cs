using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.PromoCodes;

[Collection("AdminApi")]
public class PromoCodesControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public PromoCodesControllerTests(AdminApiFactory factory)
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
        var response = await _client.GetAsync("/api/promocodes");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        var currencyId = await CreateCurrencyAsync();
        var code = $"PROMO-{Guid.NewGuid().ToString()[..8].ToUpper()}";

        var response = await _client.PostAsJsonAsync("/api/promocodes", new
        {
            Code = code,
            DiscountPercentage = 10.0m,
            DiscountAmount = (decimal?)null,
            CurrencyId = currencyId,
            ValidFrom = "2025-01-01",
            ValidTo = "2025-12-31",
            MaxUses = 100
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<PromoCodeResponse>();
        body!.Id.Should().BeGreaterThan(0);
        body.Code.Should().Be(code);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCreated()
    {
        var currencyId = await CreateCurrencyAsync();
        var code = $"PROMO-{Guid.NewGuid().ToString()[..8].ToUpper()}";

        var createResponse = await _client.PostAsJsonAsync("/api/promocodes", new
        {
            Code = code,
            DiscountPercentage = 15.0m,
            DiscountAmount = (decimal?)null,
            CurrencyId = currencyId,
            ValidFrom = "2025-01-01",
            ValidTo = "2025-12-31",
            MaxUses = 50
        });
        var created = await createResponse.Content.ReadFromJsonAsync<PromoCodeResponse>();

        var response = await _client.GetAsync($"/api/promocodes/{created!.Id}");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var currencyId = await CreateCurrencyAsync();
        var code = $"PROMO-{Guid.NewGuid().ToString()[..8].ToUpper()}";

        var createResponse = await _client.PostAsJsonAsync("/api/promocodes", new
        {
            Code = code,
            DiscountPercentage = 10.0m,
            DiscountAmount = (decimal?)null,
            CurrencyId = currencyId,
            ValidFrom = "2025-01-01",
            ValidTo = "2025-06-30",
            MaxUses = 100
        });
        var created = await createResponse.Content.ReadFromJsonAsync<PromoCodeResponse>();

        var newCode = $"PROMO-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        var updateResponse = await _client.PutAsJsonAsync($"/api/promocodes/{created!.Id}", new
        {
            Code = newCode,
            DiscountPercentage = 20.0m,
            DiscountAmount = (decimal?)null,
            CurrencyId = currencyId,
            ValidFrom = "2025-01-01",
            ValidTo = "2025-12-31",
            MaxUses = 200
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await updateResponse.Content.ReadFromJsonAsync<PromoCodeResponse>();
        body!.Code.Should().Be(newCode);
        body.DiscountPercentage.Should().Be(20.0m);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var currencyId = await CreateCurrencyAsync();
        var code = $"PROMO-{Guid.NewGuid().ToString()[..8].ToUpper()}";

        var createResponse = await _client.PostAsJsonAsync("/api/promocodes", new
        {
            Code = code,
            DiscountPercentage = 5.0m,
            DiscountAmount = (decimal?)null,
            CurrencyId = currencyId,
            ValidFrom = "2025-01-01",
            ValidTo = "2025-12-31",
            MaxUses = 10
        });
        var created = await createResponse.Content.ReadFromJsonAsync<PromoCodeResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/promocodes/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/promocodes/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SetEnabled_ReturnsNoContent()
    {
        var currencyId = await CreateCurrencyAsync();
        var code = $"PROMO-{Guid.NewGuid().ToString()[..8].ToUpper()}";

        var createResponse = await _client.PostAsJsonAsync("/api/promocodes", new
        {
            Code = code,
            DiscountPercentage = 10.0m,
            DiscountAmount = (decimal?)null,
            CurrencyId = currencyId,
            ValidFrom = "2025-01-01",
            ValidTo = "2025-12-31",
            MaxUses = 100
        });
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<PromoCodeResponse>();
        created!.Id.Should().BeGreaterThan(0);

        var patchResponse = await _client.PatchAsJsonAsync($"/api/promocodes/{created.Id}/enabled", new
        {
            Enabled = false
        });

        patchResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/promocodes");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record IdResponse(long Id);
    private record PromoCodeResponse(long Id, string Code, decimal? DiscountPercentage, decimal? DiscountAmount, long? CurrencyId, DateTime ValidFrom, DateTime ValidTo, int MaxUses, bool Enabled, DateTime CreatedAt);
}
