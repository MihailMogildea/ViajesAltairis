using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ViajesAltairis.Client.Api.Tests.Helpers;

public static class AuthHelper
{
    private const string SecretKey = "TestSecretKeyForJwtTokenGeneration1234567890!";
    private const string Issuer = "ViajesAltairis.Test";
    private const string Audience = "ViajesAltairis.Test";

    public static string GenerateClientToken(long userId = 8, string email = "client1@example.com")
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, "Client"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
