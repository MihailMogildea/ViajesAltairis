namespace ViajesAltairis.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(long userId, string email, string userType);
}
