using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Domain.Interfaces;

public interface IHotelRepository : IRepository<Hotel>
{
    Task<IReadOnlyList<Hotel>> GetByCityIdAsync(long cityId, CancellationToken cancellationToken = default);
    Task<Hotel?> GetWithDetailsAsync(long id, CancellationToken cancellationToken = default);
}
