namespace ViajesAltairis.AdminApi.Auth;

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = "ViajesAltairis.AdminApi";
    public string Audience { get; set; } = "ViajesAltairis.AdminWeb";
    public int ExpirationHours { get; set; } = 8;
}
