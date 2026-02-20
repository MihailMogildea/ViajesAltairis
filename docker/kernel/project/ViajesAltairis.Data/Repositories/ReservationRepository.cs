using Microsoft.EntityFrameworkCore;
using ViajesAltairis.Data.Context;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Data.Repositories;

public class ReservationRepository : Repository<Reservation>, IReservationRepository
{
    public ReservationRepository(AppDbContext context) : base(context) { }

    public async Task<Reservation?> GetByCodeAsync(string reservationCode, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(r => r.ReservationCode == reservationCode, cancellationToken);
    }

    public async Task<Reservation?> GetWithLinesAsync(long id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.ReservationLines).ThenInclude(rl => rl.ReservationGuests)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Reservation>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.BookedByUserId == userId || r.OwnerUserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
