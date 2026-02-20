using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.UserSubscriptions;

[Collection("AdminApi")]
public class UserSubscriptionsControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public UserSubscriptionsControllerTests(AdminApiFactory factory)
    {
        _factory = factory;
        _client = TestAuthHelper.CreateAuthenticatedClient(factory);
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/usersubscriptions");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Assign_Returns201()
    {
        var subscriptionTypeId = await CreateSubscriptionTypeChainAsync();

        var response = await _client.PostAsJsonAsync("/api/usersubscriptions", new
        {
            UserId = 1L,
            SubscriptionTypeId = subscriptionTypeId,
            StartDate = "2025-01-01",
            EndDate = "2025-12-31"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body!.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Assign_ThenDelete_Returns204()
    {
        var subscriptionTypeId = await CreateSubscriptionTypeChainAsync();

        var assignResponse = await _client.PostAsJsonAsync("/api/usersubscriptions", new
        {
            UserId = 1L,
            SubscriptionTypeId = subscriptionTypeId,
            StartDate = "2025-06-01",
            EndDate = "2026-05-31"
        });
        var created = await assignResponse.Content.ReadFromJsonAsync<IdResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/usersubscriptions/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/usersubscriptions");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<long> CreateCurrencyAsync()
    {
        var iso = TestAuthHelper.UniqueIso(3);
        var response = await _client.PostAsJsonAsync("/api/currencies", new { IsoCode = iso, Name = $"Cur {iso}", Symbol = "$" });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
    }

    private async Task<long> CreateSubscriptionTypeAsync(long currencyId)
    {
        var response = await _client.PostAsJsonAsync("/api/subscriptiontypes", new
        {
            Name = $"Sub {Guid.NewGuid().ToString()[..6]}",
            PricePerMonth = 29.99m,
            Discount = 5.0m,
            CurrencyId = currencyId
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<IdResponse>())!.Id;
    }

    private async Task<long> CreateSubscriptionTypeChainAsync()
    {
        var currencyId = await CreateCurrencyAsync();
        return await CreateSubscriptionTypeAsync(currencyId);
    }

    private record IdResponse(long Id);
}
