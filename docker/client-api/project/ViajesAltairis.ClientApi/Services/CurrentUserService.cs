using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.ClientApi.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public long? UserId
    {
        get
        {
            var sub = _httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                   ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(sub, out var id) ? id : null;
        }
    }

    public string? Email =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Email)
        ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

    public string? UserType =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

    public long LanguageId
    {
        get
        {
            var lang = _httpContextAccessor.HttpContext?.Request.Headers.AcceptLanguage.FirstOrDefault();
            return lang?.StartsWith("es", StringComparison.OrdinalIgnoreCase) == true ? 2 : 1;
        }
    }

    public string CurrencyCode =>
        _httpContextAccessor.HttpContext?.Request.Headers["X-Currency"].FirstOrDefault() ?? "EUR";
}
