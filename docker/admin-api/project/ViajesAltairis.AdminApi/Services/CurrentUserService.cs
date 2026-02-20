using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.AdminApi.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public long? UserId
    {
        get
        {
            var sub = User?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                      ?? User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(sub, out var id) ? id : null;
        }
    }

    public string? Email => User?.FindFirstValue(JwtRegisteredClaimNames.Email)
                            ?? User?.FindFirstValue(ClaimTypes.Email);

    public string? UserType
    {
        get
        {
            var typeId = User?.FindFirstValue("user_type_id");
            return typeId switch
            {
                "1" => "admin",
                "2" => "manager",
                "3" => "agent",
                "4" => "hotel_staff",
                _ => null
            };
        }
    }

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
