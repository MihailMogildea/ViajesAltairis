using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Infrastructure.Security;

public class PasswordService : IPasswordService
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 10);
    }

    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
