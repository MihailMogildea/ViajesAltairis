using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ViajesAltairis.ExternalClientApi.Configuration;

namespace ViajesAltairis.ExternalClientApi.Authentication;

public class JwtTokenService
{
    private readonly JwtSettings _settings;

    public JwtTokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public string GenerateToken(long userId, string email, long businessPartnerId, string partnerName)
    {
        var claims = new[]
        {
            new Claim("userId", userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim("businessPartnerId", businessPartnerId.ToString()),
            new Claim("partnerName", partnerName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_settings.ExpirationHours),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
