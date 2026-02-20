using Microsoft.EntityFrameworkCore;
using ViajesAltairis.Data.Context;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Data.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}
