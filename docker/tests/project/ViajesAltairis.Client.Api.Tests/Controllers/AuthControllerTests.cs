using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ViajesAltairis.Application.Features.Client.Auth.Commands.ForgotPassword;
using ViajesAltairis.Application.Features.Client.Auth.Commands.Login;
using ViajesAltairis.Application.Features.Client.Auth.Commands.Register;
using ViajesAltairis.Application.Features.Client.Auth.Commands.ResetPassword;
using ViajesAltairis.Client.Api.Tests.Fixtures;

namespace ViajesAltairis.Client.Api.Tests.Controllers;

public class AuthControllerTests : IClassFixture<ClientApiFactory>
{
    private readonly HttpClient _client;
    private readonly ClientApiFactory _factory;

    public AuthControllerTests(ClientApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenValid()
    {
        _factory.MockMediator
            .Send(Arg.Any<LoginCommand>(), Arg.Any<CancellationToken>())
            .Returns(new LoginResponse { Token = "jwt-token", ExpiresAt = DateTime.UtcNow.AddHours(1) });

        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new { Email = "test@test.com", Password = "password123" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_Returns403_WhenUnauthorized()
    {
        _factory.MockMediator
            .Send(Arg.Any<LoginCommand>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new UnauthorizedAccessException("Invalid email or password."));

        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new { Email = "bad@test.com", Password = "wrong" });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Register_ReturnsOk_WhenValid()
    {
        _factory.MockMediator
            .Send(Arg.Any<RegisterCommand>(), Arg.Any<CancellationToken>())
            .Returns(new RegisterResponse { UserId = 99, Token = "jwt-token" });

        var response = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = "new@test.com",
            Password = "password123",
            FirstName = "Test",
            LastName = "User",
            PreferredLanguageId = 1L,
            PreferredCurrencyId = 1L
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_Returns400_WhenDuplicate()
    {
        _factory.MockMediator
            .Send(Arg.Any<RegisterCommand>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("A user with this email already exists."));

        var response = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = "dup@test.com",
            Password = "password123",
            FirstName = "Test",
            LastName = "User",
            PreferredLanguageId = 1L,
            PreferredCurrencyId = 1L
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ForgotPassword_ReturnsOk()
    {
        _factory.MockMediator
            .Send(Arg.Any<ForgotPasswordCommand>(), Arg.Any<CancellationToken>())
            .Returns(Unit.Value);

        var response = await _client.PostAsJsonAsync("/api/auth/forgot-password",
            new { Email = "test@test.com" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ResetPassword_ReturnsOk()
    {
        _factory.MockMediator
            .Send(Arg.Any<ResetPasswordCommand>(), Arg.Any<CancellationToken>())
            .Returns(Unit.Value);

        var response = await _client.PostAsJsonAsync("/api/auth/reset-password",
            new { Token = "some-token", NewPassword = "newpass123" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
