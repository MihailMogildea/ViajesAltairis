namespace ViajesAltairis.Application.Interfaces;

public interface IScheduledApiClient
{
    Task TriggerJobAsync(string jobKey, CancellationToken cancellationToken = default);
    Task ReloadSchedulesAsync(CancellationToken cancellationToken = default);
}
