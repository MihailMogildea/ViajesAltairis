using Dapper;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.ScheduledApi.Jobs;

public class ProviderUpdateJob
{
    private readonly IDbConnectionFactory _db;
    private readonly ILogger<ProviderUpdateJob> _logger;

    public ProviderUpdateJob(IDbConnectionFactory db, ILogger<ProviderUpdateJob> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Provider update started");

        // TODO: Query all enabled external providers
        // TODO: For each provider, call their API to sync:
        //   - Room availability
        //   - Pricing updates
        //   - Hotel information changes

        await Task.CompletedTask;

        using var connection = _db.CreateConnection();
        await connection.ExecuteAsync(
            "UPDATE job_schedule SET last_executed_at = NOW() WHERE job_key = 'provider-update'");

        _logger.LogInformation("Provider update completed");
    }
}
