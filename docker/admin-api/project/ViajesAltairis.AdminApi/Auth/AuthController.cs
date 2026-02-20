using System.Data;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.AdminApi.Auth.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.AdminApi.Auth;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IDbConnectionFactory _db;
    private readonly IPasswordService _passwordService;
    private readonly JwtTokenService _jwtTokenService;

    public AuthController(IDbConnectionFactory db, IPasswordService passwordService, JwtTokenService jwtTokenService)
    {
        _db = db;
        _passwordService = passwordService;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        using var connection = _db.CreateConnection();

        var user = await connection.QuerySingleOrDefaultAsync<UserRow>(
            """
            SELECT id, email, password_hash, first_name, last_name,
                   user_type_id, provider_id, business_partner_id, enabled
            FROM user
            WHERE email = @Email
            """,
            new { request.Email });

        if (user is null)
            return Unauthorized(new { error = "Invalid email or password." });

        if (!user.Enabled)
            return Unauthorized(new { error = "Account is disabled." });

        // user_type_id=5 is 'client' — not allowed in admin panel
        if (user.UserTypeId == 5)
            return Unauthorized(new { error = "Invalid email or password." });

        if (!_passwordService.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { error = "Invalid email or password." });

        var name = $"{user.FirstName} {user.LastName}";
        var token = _jwtTokenService.GenerateToken(
            user.Id, user.Email, name, user.UserTypeId, user.ProviderId, user.BusinessPartnerId);

        return Ok(new LoginResponse(token));
    }

    private sealed class UserRow
    {
        public long Id { get; init; }
        public string Email { get; init; } = default!;
        public string PasswordHash { get; init; } = default!;
        public string FirstName { get; init; } = default!;
        public string LastName { get; init; } = default!;
        public long UserTypeId { get; init; }
        public long? ProviderId { get; init; }
        public long? BusinessPartnerId { get; init; }
        public bool Enabled { get; init; }
    }
}
