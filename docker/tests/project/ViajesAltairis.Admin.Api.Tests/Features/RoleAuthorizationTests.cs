using System.Net;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features;

[Collection("AdminApi")]
public class RoleAuthorizationTests
{
    private readonly AdminApiFactory _factory;

    public RoleAuthorizationTests(AdminApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task UnauthenticatedRequest_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/currencies");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AuthEndpoint_SkipsRoleCheck()
    {
        var client = _factory.CreateClient();
        // Auth endpoint is [AllowAnonymous] and skipped by middleware
        var response = await client.PostAsync("/api/auth/login", null);
        // Should not be 401 from middleware — it will be 400/415 from missing body
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    // --- Admin (user_type_id=1) has access to all sections ---

    [Theory]
    [InlineData("/api/currencies")]
    [InlineData("/api/hotels")]
    [InlineData("/api/providers")]
    [InlineData("/api/reservations")]
    [InlineData("/api/users")]
    [InlineData("/api/businesspartners")]
    [InlineData("/api/seasonalmargins")]
    [InlineData("/api/subscriptiontypes")]
    [InlineData("/api/invoices")]
    [InlineData("/api/hotelblackouts")]
    [InlineData("/api/reviews")]
    [InlineData("/api/auditlogs")]
    public async Task AdminRole_HasAccess(string path)
    {
        var client = TestAuthHelper.CreateAuthenticatedClient(_factory, userTypeId: 1);
        var response = await client.GetAsync(path);
        // Admin should pass middleware — not 401 or 403
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    // --- Manager (user_type_id=2) ---

    [Theory]
    [InlineData("/api/hotels")]
    [InlineData("/api/reservations")]
    [InlineData("/api/users")]
    [InlineData("/api/businesspartners")]
    [InlineData("/api/seasonalmargins")]
    [InlineData("/api/subscriptiontypes")]
    [InlineData("/api/invoices")]
    [InlineData("/api/hotelblackouts")]
    [InlineData("/api/reviews")]
    public async Task ManagerRole_GetAccess(string path)
    {
        var client = TestAuthHelper.CreateAuthenticatedClient(_factory, userTypeId: 2);
        var response = await client.GetAsync(path);
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData("/api/currencies")]
    [InlineData("/api/providers")]
    [InlineData("/api/auditlogs")]
    public async Task ManagerRole_DeniedSections(string path)
    {
        var client = TestAuthHelper.CreateAuthenticatedClient(_factory, userTypeId: 2);
        var response = await client.GetAsync(path);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData("/api/hotels")]
    [InlineData("/api/users")]
    [InlineData("/api/businesspartners")]
    [InlineData("/api/seasonalmargins")]
    [InlineData("/api/subscriptiontypes")]
    [InlineData("/api/invoices")]
    public async Task ManagerRole_ReadOnlySections_PostDenied(string path)
    {
        var client = TestAuthHelper.CreateAuthenticatedClient(_factory, userTypeId: 2);
        var response = await client.PostAsync(path, new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // --- Agent (user_type_id=3) ---

    [Theory]
    [InlineData("/api/hotels")]
    [InlineData("/api/reservations")]
    [InlineData("/api/businesspartners")]
    [InlineData("/api/subscriptiontypes")]
    [InlineData("/api/invoices")]
    public async Task AgentRole_AllowedSections(string path)
    {
        var client = TestAuthHelper.CreateAuthenticatedClient(_factory, userTypeId: 3);
        var response = await client.GetAsync(path);
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData("/api/currencies")]
    [InlineData("/api/providers")]
    [InlineData("/api/users")]
    [InlineData("/api/hotelblackouts")]
    [InlineData("/api/reviews")]
    [InlineData("/api/auditlogs")]
    public async Task AgentRole_DeniedSections(string path)
    {
        var client = TestAuthHelper.CreateAuthenticatedClient(_factory, userTypeId: 3);
        var response = await client.GetAsync(path);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // --- Hotel Staff (user_type_id=4) ---

    [Theory]
    [InlineData("/api/hotels")]
    [InlineData("/api/reservations")]
    [InlineData("/api/invoices")]
    [InlineData("/api/hotelblackouts")]
    [InlineData("/api/reviews")]
    public async Task HotelStaffRole_AllowedSections(string path)
    {
        var client = TestAuthHelper.CreateAuthenticatedClient(_factory, userTypeId: 4);
        var response = await client.GetAsync(path);
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData("/api/currencies")]
    [InlineData("/api/providers")]
    [InlineData("/api/users")]
    [InlineData("/api/businesspartners")]
    [InlineData("/api/seasonalmargins")]
    [InlineData("/api/subscriptiontypes")]
    [InlineData("/api/auditlogs")]
    public async Task HotelStaffRole_DeniedSections(string path)
    {
        var client = TestAuthHelper.CreateAuthenticatedClient(_factory, userTypeId: 4);
        var response = await client.GetAsync(path);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // --- Web translations public endpoint ---

    [Fact]
    public async Task WebTranslations_PublicGet_AllowsAnonymous()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/webtranslations");
        // Should not be 401 — public GET is allowed by middleware
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }
}
