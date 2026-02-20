using Microsoft.EntityFrameworkCore;
using ViajesAltairis.Data.Context;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Data.Repositories;

public class HotelRepository : Repository<Hotel>, IHotelRepository
{
    public HotelRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Hotel>> GetByCityIdAsync(long cityId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(h => h.CityId == cityId && h.Enabled)
            .ToListAsync(cancellationToken);
    }

    public async Task<Hotel?> GetWithDetailsAsync(long id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(h => h.City)
            .Include(h => h.HotelImages)
            .Include(h => h.HotelAmenities).ThenInclude(ha => ha.Amenity)
            .Include(h => h.HotelProviders).ThenInclude(hp => hp.HotelProviderRoomTypes)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }
}
