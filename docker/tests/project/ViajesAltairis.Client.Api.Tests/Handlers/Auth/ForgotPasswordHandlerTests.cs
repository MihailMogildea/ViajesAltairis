using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Auth.Commands.ForgotPassword;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Client.Api.Tests.Handlers.Auth;

public class ForgotPasswordHandlerTests
{
    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly ICacheService _cacheService = Substitute.For<ICacheService>();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly IConfiguration _configuration;
    private readonly ForgotPasswordHandler _handler;

    public ForgotPasswordHandlerTests()
    {
        var configData = new Dictionary<string, string?> { ["App:BaseUrl"] = "https://test.com" };
        _configuration = new ConfigurationBuilder().AddInMemoryCollection(configData).Build();
        _handler = new ForgotPasswordHandler(_userRepo, _cacheService, _emailService, _configuration);
    }

    [Fact]
    public async Task Handle_ValidUser_SendsEmail()
    {
        var user = new User { Id = 8, Email = "client1@example.com", PasswordHash = "hash", UserTypeId = 5, Enabled = true, FirstName = "J", LastName = "M" };
        _userRepo.GetByEmailAsync("client1@example.com", Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(new ForgotPasswordCommand { Email = "client1@example.com" }, CancellationToken.None);

        result.Should().Be(Unit.Value);
        await _cacheService.Received(1).SetAsync(Arg.Is<string>(s => s.StartsWith("pwd-reset:")), 8L, Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>());
        await _emailService.Received(1).SendEmailAsync("client1@example.com", Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UserNotFound_SilentlySucceeds()
    {
        _userRepo.GetByEmailAsync("nobody@test.com", Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _handler.Handle(new ForgotPasswordCommand { Email = "nobody@test.com" }, CancellationToken.None);

        result.Should().Be(Unit.Value);
        await _emailService.DidNotReceive().SendEmailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
