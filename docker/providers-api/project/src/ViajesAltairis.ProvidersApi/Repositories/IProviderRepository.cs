namespace ViajesAltairis.ProvidersApi.Repositories;

public interface IProviderRepository
{
    Task<IEnumerable<dynamic>> GetAllEnabledAsync();
    Task<dynamic?> GetByIdAsync(long id);
    Task<dynamic?> GetByNameAsync(string name);
    Task<long> InsertAsync(string name, long typeId, decimal margin);
    Task<bool> TrySetSyncStatusAsync(long id, string newStatus, string? expectedCurrentStatus);
    Task SetSyncCompletedAsync(long id);
    Task SetSyncFailedAsync(long id);
}
