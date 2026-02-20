using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.Auth;

[Collection("AdminApi")]
public class AuthControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public AuthControllerTests(AdminApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = "admin@test.com",
            Password = "password123"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<TokenResponse>();
        body!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = "admin@test.com",
            Password = "wrongpassword"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = "nonexistent@test.com",
            Password = "password123"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithDisabledAccount_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = "disabled@test.com",
            Password = "password123"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithClientUserType_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = "client@test.com",
            Password = "password123"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record TokenResponse(string Token);
}
