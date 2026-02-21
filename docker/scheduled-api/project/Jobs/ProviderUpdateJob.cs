using Dapper;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.ScheduledApi.Jobs;

public class ProviderUpdateJob
{
    private readonly IDbConnectionFactory _db;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProviderUpdateJob> _logger;

    public ProviderUpdateJob(IDbConnectionFactory db, IHttpClientFactory httpClientFactory,
        IConfiguration configuration, ILogger<ProviderUpdateJob> logger)
    {
        _db = db;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Provider update started");

        var baseUrl = _configuration["ProvidersApi:BaseUrl"];
        if (string.IsNullOrEmpty(baseUrl))
        {
            _logger.LogError("ProvidersApi:BaseUrl is not configured — skipping provider sync");
            return;
        }

        using var connection = _db.CreateConnection();

        var providers = (await connection.QueryAsync<(long Id, string Name)>(
            "SELECT id, name FROM provider WHERE type_id = 2 AND enabled = TRUE")).ToList();

        _logger.LogInformation("Found {Count} enabled external providers to sync", providers.Count);

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(baseUrl);

        foreach (var provider in providers)
        {
            try
            {
                var response = await client.PostAsync($"/api/providers/{provider.Id}/sync", null, cancellationToken);
                if (response.IsSuccessStatusCode)
                    _logger.LogInformation("Sync triggered for provider {Name} (id={Id})", provider.Name, provider.Id);
                else
                    _logger.LogWarning("Sync trigger failed for provider {Name} (id={Id}): {Status}",
                        provider.Name, provider.Id, response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error triggering sync for provider {Name} (id={Id})", provider.Name, provider.Id);
            }
        }

        await connection.ExecuteAsync(
            "UPDATE job_schedule SET last_executed_at = NOW() WHERE job_key = 'provider-update'");

        _logger.LogInformation("Provider update completed");
    }
}
