namespace ViajesAltairis.Domain.Interfaces;

/// <summary>
/// Repository interface for entities that don't extend BaseEntity (BoardType, EmailTemplate, HotelProviderRoomTypeBoard).
/// </summary>
public interface ISimpleRepository<T> where T : class
{
    Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}
