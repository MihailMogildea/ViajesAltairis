using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Admin.Api.Tests.Infrastructure;

public class FakePasswordService : IPasswordService
{
    public const string HashPrefix = "HASHED:";

    public string Hash(string password) => HashPrefix + password;

    public bool Verify(string password, string hash) => hash == HashPrefix + password;
}
