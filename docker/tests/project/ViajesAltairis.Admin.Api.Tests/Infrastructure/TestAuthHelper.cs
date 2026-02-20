using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ViajesAltairis.Admin.Api.Tests.Infrastructure;

public static class TestAuthHelper
{
    public const string TestSecretKey = "TestSecretKeyForAdminApiIntegrationTests_AtLeast32Chars!";
    public const string TestIssuer = "ViajesAltairis.AdminApi";
    public const string TestAudience = "ViajesAltairis.AdminWeb";

    private static int _counter;

    /// <summary>
    /// Generates a unique ISO code of the specified length using an atomic counter.
    /// Uses base-36 (0-9, A-Z) for maximum uniqueness in short strings.
    /// </summary>
    public static string UniqueIso(int length)
    {
        var n = Interlocked.Increment(ref _counter);
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var result = new char[length];
        for (var i = length - 1; i >= 0; i--)
        {
            result[i] = chars[n % chars.Length];
            n /= chars.Length;
        }
        return new string(result);
    }

    public static string GenerateToken(
        long userId = 1,
        string email = "admin@test.com",
        string name = "Test Admin",
        long userTypeId = 1,
        long? providerId = null,
        long? businessPartnerId = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new("name", name),
            new("user_type_id", userTypeId.ToString()),
        };

        if (providerId.HasValue)
            claims.Add(new Claim("provider_id", providerId.Value.ToString()));
        if (businessPartnerId.HasValue)
            claims.Add(new Claim("business_partner_id", businessPartnerId.Value.ToString()));

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static HttpClient CreateAuthenticatedClient(AdminApiFactory factory, long userTypeId = 1)
    {
        var client = factory.CreateClient();
        var token = GenerateToken(userTypeId: userTypeId);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
