using FluentAssertions;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Auth.Commands.ResetPassword;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Client.Api.Tests.Handlers.Auth;

public class ResetPasswordHandlerTests
{
    private readonly ICacheService _cacheService = Substitute.For<ICacheService>();
    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly IPasswordService _passwordService = Substitute.For<IPasswordService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ResetPasswordHandler _handler;

    public ResetPasswordHandlerTests()
    {
        _handler = new ResetPasswordHandler(_cacheService, _userRepo, _passwordService, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ValidToken_ResetsPassword()
    {
        _cacheService.GetAsync<long?>("pwd-reset:valid-token", Arg.Any<CancellationToken>()).Returns(8L);
        var user = new User { Id = 8, Email = "client1@example.com", PasswordHash = "old-hash", UserTypeId = 5, Enabled = true, FirstName = "J", LastName = "M" };
        _userRepo.GetByIdAsync(8L, Arg.Any<CancellationToken>()).Returns(user);
        _passwordService.Hash("newpass123").Returns("new-hash");

        await _handler.Handle(new ResetPasswordCommand { Token = "valid-token", NewPassword = "newpass123" }, CancellationToken.None);

        user.PasswordHash.Should().Be("new-hash");
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveAsync("pwd-reset:valid-token", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InvalidToken_ThrowsInvalidOperation()
    {
        _cacheService.GetAsync<long?>("pwd-reset:bad-token", Arg.Any<CancellationToken>()).Returns((long?)null);

        var act = () => _handler.Handle(new ResetPasswordCommand { Token = "bad-token", NewPassword = "newpass123" }, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*expired*");
    }
}
