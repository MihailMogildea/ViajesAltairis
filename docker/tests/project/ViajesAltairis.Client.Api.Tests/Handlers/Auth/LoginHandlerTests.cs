using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Auth.Commands.Login;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Client.Api.Tests.Handlers.Auth;

public class LoginHandlerTests
{
    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly IPasswordService _passwordService = Substitute.For<IPasswordService>();
    private readonly IJwtTokenService _jwtTokenService = Substitute.For<IJwtTokenService>();
    private readonly IConfiguration _configuration;
    private readonly LoginHandler _handler;

    public LoginHandlerTests()
    {
        var configData = new Dictionary<string, string?> { ["Jwt:ExpirationInMinutes"] = "60" };
        _configuration = new ConfigurationBuilder().AddInMemoryCollection(configData).Build();
        _handler = new LoginHandler(_userRepo, _passwordService, _jwtTokenService, _configuration);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        var user = new User { Id = 8, Email = "client1@example.com", PasswordHash = "hash", UserTypeId = 5, Enabled = true, FirstName = "Juan", LastName = "M" };
        _userRepo.GetByEmailAsync("client1@example.com", Arg.Any<CancellationToken>()).Returns(user);
        _passwordService.Verify("password123", "hash").Returns(true);
        _jwtTokenService.GenerateToken(8, "client1@example.com", "Client").Returns("jwt-token");

        var result = await _handler.Handle(new LoginCommand { Email = "client1@example.com", Password = "password123" }, CancellationToken.None);

        result.Token.Should().Be("jwt-token");
        result.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(60), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsUnauthorized()
    {
        var user = new User { Id = 8, Email = "client1@example.com", PasswordHash = "hash", UserTypeId = 5, Enabled = true, FirstName = "Juan", LastName = "M" };
        _userRepo.GetByEmailAsync("client1@example.com", Arg.Any<CancellationToken>()).Returns(user);
        _passwordService.Verify("wrong", "hash").Returns(false);

        var act = () => _handler.Handle(new LoginCommand { Email = "client1@example.com", Password = "wrong" }, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_NonClientUserType_ThrowsUnauthorized()
    {
        var user = new User { Id = 1, Email = "admin@test.com", PasswordHash = "hash", UserTypeId = 1, Enabled = true, FirstName = "Admin", LastName = "A" };
        _userRepo.GetByEmailAsync("admin@test.com", Arg.Any<CancellationToken>()).Returns(user);

        var act = () => _handler.Handle(new LoginCommand { Email = "admin@test.com", Password = "password123" }, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_DisabledUser_ThrowsUnauthorized()
    {
        var user = new User { Id = 8, Email = "disabled@test.com", PasswordHash = "hash", UserTypeId = 5, Enabled = false, FirstName = "Dis", LastName = "D" };
        _userRepo.GetByEmailAsync("disabled@test.com", Arg.Any<CancellationToken>()).Returns(user);

        var act = () => _handler.Handle(new LoginCommand { Email = "disabled@test.com", Password = "password123" }, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsUnauthorized()
    {
        _userRepo.GetByEmailAsync("nobody@test.com", Arg.Any<CancellationToken>()).Returns((User?)null);

        var act = () => _handler.Handle(new LoginCommand { Email = "nobody@test.com", Password = "password123" }, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
