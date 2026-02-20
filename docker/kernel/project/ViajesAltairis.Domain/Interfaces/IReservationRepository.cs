using ViajesAltairis.Domain.Entities;

namespace ViajesAltairis.Domain.Interfaces;

public interface IReservationRepository : IRepository<Reservation>
{
    Task<Reservation?> GetByCodeAsync(string reservationCode, CancellationToken cancellationToken = default);
    Task<Reservation?> GetWithLinesAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Reservation>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);
}
