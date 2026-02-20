using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
