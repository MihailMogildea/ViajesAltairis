using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.ExternalClient.Auth.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.ExternalClient.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<LoginResult>;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IDbConnectionFactory _db;
    private readonly IPasswordService _passwordService;

    public LoginHandler(IDbConnectionFactory db, IPasswordService passwordService)
    {
        _db = db;
        _passwordService = passwordService;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();

        const string sql = @"
            SELECT u.id, u.email, u.password_hash, u.enabled,
                   bp.id AS business_partner_id, bp.company_name, bp.enabled AS partner_enabled
            FROM user u
            JOIN user_type ut ON ut.id = u.user_type_id
            LEFT JOIN business_partner bp ON bp.id = u.business_partner_id
            WHERE u.email = @Email AND ut.name = 'agent'";

        var user = await connection.QuerySingleOrDefaultAsync<UserLoginRow>(sql, new { request.Email });

        if (user is null)
            throw new UnauthorizedAccessException("Invalid credentials.");

        if (!_passwordService.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        if (!user.Enabled)
            throw new UnauthorizedAccessException("User account is disabled.");

        if (user.BusinessPartnerId is null)
            throw new UnauthorizedAccessException("User is not associated with a business partner.");

        if (!user.PartnerEnabled)
            throw new UnauthorizedAccessException("Business partner account is disabled.");

        return new LoginResult(user.Id, user.Email, user.BusinessPartnerId.Value, user.CompanyName!);
    }

    private record UserLoginRow(
        long Id,
        string Email,
        string PasswordHash,
        bool Enabled,
        long? BusinessPartnerId,
        string? CompanyName,
        bool PartnerEnabled);
}
