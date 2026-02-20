using ViajesAltairis.Application.Features.ExternalClient.Auth.Commands;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.ExternalClient.Api.Tests.Helpers;

namespace ViajesAltairis.ExternalClient.Api.Tests.Auth;

public class LoginHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _db;
    private readonly IPasswordService _passwordService;
    private readonly LoginHandler _handler;

    public LoginHandlerTests()
    {
        _db = new SqliteTestDatabase();
        _db.CreateSchema().SeedReferenceData();
        _passwordService = Substitute.For<IPasswordService>();
        _handler = new LoginHandler(_db, _passwordService);

        _db.Execute("""
            INSERT INTO business_partner (id, company_name, enabled) VALUES (1, 'Acme Travel', 1);
            INSERT INTO user (id, email, password_hash, enabled, user_type_id, business_partner_id, first_name, last_name)
                VALUES (1, 'agent@acme.com', 'hashed_pw', 1, 3, 1, 'Jane', 'Agent');
            """);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsLoginResult()
    {
        _passwordService.Verify("correct_password", "hashed_pw").Returns(true);

        var result = await _handler.Handle(new LoginCommand("agent@acme.com", "correct_password"), CancellationToken.None);

        result.UserId.Should().Be(1);
        result.Email.Should().Be("agent@acme.com");
        result.BusinessPartnerId.Should().Be(1);
        result.PartnerName.Should().Be("Acme Travel");
    }

    [Fact]
    public async Task Login_UserNotFound_ThrowsUnauthorized()
    {
        var act = () => _handler.Handle(new LoginCommand("unknown@test.com", "pw"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials.");
    }

    [Fact]
    public async Task Login_WrongPassword_ThrowsUnauthorized()
    {
        _passwordService.Verify("wrong", "hashed_pw").Returns(false);

        var act = () => _handler.Handle(new LoginCommand("agent@acme.com", "wrong"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials.");
    }

    [Fact]
    public async Task Login_UserDisabled_ThrowsUnauthorized()
    {
        _db.Execute("UPDATE user SET enabled = 0 WHERE id = 1");
        _passwordService.Verify("correct_password", "hashed_pw").Returns(true);

        var act = () => _handler.Handle(new LoginCommand("agent@acme.com", "correct_password"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("User account is disabled.");
    }

    [Fact]
    public async Task Login_NoBusinessPartner_ThrowsUnauthorized()
    {
        _db.Execute("""
            INSERT INTO user (id, email, password_hash, enabled, user_type_id, business_partner_id, first_name, last_name)
                VALUES (2, 'orphan@test.com', 'hashed_pw', 1, 3, NULL, 'No', 'Partner');
            """);
        _passwordService.Verify("pw", "hashed_pw").Returns(true);

        var act = () => _handler.Handle(new LoginCommand("orphan@test.com", "pw"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("User is not associated with a business partner.");
    }

    [Fact]
    public async Task Login_PartnerDisabled_ThrowsUnauthorized()
    {
        _db.Execute("UPDATE business_partner SET enabled = 0 WHERE id = 1");
        _passwordService.Verify("correct_password", "hashed_pw").Returns(true);

        var act = () => _handler.Handle(new LoginCommand("agent@acme.com", "correct_password"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Business partner account is disabled.");
    }

    public void Dispose() => _db.Dispose();
}
