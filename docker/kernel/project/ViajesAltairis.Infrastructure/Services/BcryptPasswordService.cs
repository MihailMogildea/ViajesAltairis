using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Infrastructure.Services;

public class BcryptPasswordService : IPasswordService
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 10);

    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
