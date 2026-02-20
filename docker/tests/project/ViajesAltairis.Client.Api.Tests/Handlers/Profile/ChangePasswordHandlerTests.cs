using FluentAssertions;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Profile.Commands.ChangePassword;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Client.Api.Tests.Handlers.Profile;

public class ChangePasswordHandlerTests
{
    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly IPasswordService _passwordService = Substitute.For<IPasswordService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ChangePasswordHandler _handler;

    public ChangePasswordHandlerTests()
    {
        _currentUser.UserId.Returns(8L);
        _handler = new ChangePasswordHandler(_userRepo, _currentUser, _passwordService, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ValidCurrentPassword_ChangesPassword()
    {
        var user = new User { Id = 8, Email = "client1@example.com", PasswordHash = "old-hash", FirstName = "J", LastName = "M", UserTypeId = 5, Enabled = true };
        _userRepo.GetByIdAsync(8L, Arg.Any<CancellationToken>()).Returns(user);
        _passwordService.Verify("current123", "old-hash").Returns(true);
        _passwordService.Hash("newpass123").Returns("new-hash");

        await _handler.Handle(new ChangePasswordCommand { CurrentPassword = "current123", NewPassword = "newpass123" }, CancellationToken.None);

        user.PasswordHash.Should().Be("new-hash");
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WrongCurrentPassword_ThrowsUnauthorized()
    {
        var user = new User { Id = 8, Email = "client1@example.com", PasswordHash = "old-hash", FirstName = "J", LastName = "M", UserTypeId = 5, Enabled = true };
        _userRepo.GetByIdAsync(8L, Arg.Any<CancellationToken>()).Returns(user);
        _passwordService.Verify("wrong", "old-hash").Returns(false);

        var act = () => _handler.Handle(new ChangePasswordCommand { CurrentPassword = "wrong", NewPassword = "newpass123" }, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
