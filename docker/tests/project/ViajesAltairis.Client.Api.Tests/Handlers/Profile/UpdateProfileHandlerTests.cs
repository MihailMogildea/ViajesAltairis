using FluentAssertions;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Profile.Commands.UpdateProfile;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Client.Api.Tests.Handlers.Profile;

public class UpdateProfileHandlerTests
{
    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdateProfileHandler _handler;

    public UpdateProfileHandlerTests()
    {
        _currentUser.UserId.Returns(8L);
        _handler = new UpdateProfileHandler(_userRepo, _currentUser, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ValidUpdate_UpdatesFields()
    {
        var user = new User { Id = 8, Email = "client1@example.com", FirstName = "Juan", LastName = "Martínez", PasswordHash = "hash", UserTypeId = 5, Enabled = true };
        _userRepo.GetByIdAsync(8L, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(new UpdateProfileCommand { FirstName = "Updated", Phone = "+34 700" }, CancellationToken.None);

        result.UserId.Should().Be(8);
        user.FirstName.Should().Be("Updated");
        user.Phone.Should().Be("+34 700");
        user.LastName.Should().Be("Martínez"); // unchanged
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsKeyNotFound()
    {
        _userRepo.GetByIdAsync(8L, Arg.Any<CancellationToken>()).Returns((User?)null);

        var act = () => _handler.Handle(new UpdateProfileCommand { FirstName = "Updated" }, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
