using FluentAssertions;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Auth.Commands.Register;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Client.Api.Tests.Handlers.Auth;

public class RegisterHandlerTests
{
    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly IPasswordService _passwordService = Substitute.For<IPasswordService>();
    private readonly IJwtTokenService _jwtTokenService = Substitute.For<IJwtTokenService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly RegisterHandler _handler;

    public RegisterHandlerTests()
    {
        _handler = new RegisterHandler(_userRepo, _passwordService, _jwtTokenService, _unitOfWork);
    }

    [Fact]
    public async Task Handle_NewUser_CreatesAndReturnsToken()
    {
        _userRepo.GetByEmailAsync("new@test.com", Arg.Any<CancellationToken>()).Returns((User?)null);
        _passwordService.Hash("password123").Returns("hashed");
        _jwtTokenService.GenerateToken(Arg.Any<long>(), "new@test.com", "Client").Returns("jwt-token");

        var result = await _handler.Handle(new RegisterCommand
        {
            Email = "new@test.com",
            Password = "password123",
            FirstName = "New",
            LastName = "User",
            PreferredLanguageId = 1,
            PreferredCurrencyId = 1
        }, CancellationToken.None);

        result.Token.Should().Be("jwt-token");
        await _userRepo.Received(1).AddAsync(Arg.Is<User>(u => u.Email == "new@test.com" && u.UserTypeId == 5), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsInvalidOperation()
    {
        var existing = new User { Id = 8, Email = "existing@test.com", PasswordHash = "hash", UserTypeId = 5, Enabled = true, FirstName = "E", LastName = "E" };
        _userRepo.GetByEmailAsync("existing@test.com", Arg.Any<CancellationToken>()).Returns(existing);

        var act = () => _handler.Handle(new RegisterCommand
        {
            Email = "existing@test.com",
            Password = "password123",
            FirstName = "New",
            LastName = "User",
            PreferredLanguageId = 1,
            PreferredCurrencyId = 1
        }, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*already exists*");
    }
}
