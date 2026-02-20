using ViajesAltairis.ProvidersApi.ExternalClients;
using ViajesAltairis.ProvidersApi.Repositories;

namespace ViajesAltairis.ProvidersApi.Services;

public class ProviderRegistrationService : IHostedService
{
    private readonly IEnumerable<IExternalProviderClient> _clients;
    private readonly IProviderRepository _providerRepo;
    private readonly ILogger<ProviderRegistrationService> _logger;

    public ProviderRegistrationService(
        IEnumerable<IExternalProviderClient> clients,
        IProviderRepository providerRepo,
        ILogger<ProviderRegistrationService> logger)
    {
        _clients = clients;
        _providerRepo = providerRepo;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var client in _clients)
        {
            try
            {
                var existing = await _providerRepo.GetByNameAsync(client.ProviderName);
                if (existing != null)
                {
                    _logger.LogInformation("Provider '{ProviderName}' already registered (id={Id})", client.ProviderName, (long)existing.id);
                    continue;
                }

                var id = await _providerRepo.InsertAsync(client.ProviderName, typeId: 2, margin: 10.00m);
                _logger.LogInformation("Registered external provider '{ProviderName}' (id={Id})", client.ProviderName, id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register provider '{ProviderName}'", client.ProviderName);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
