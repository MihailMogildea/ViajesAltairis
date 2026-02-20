using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Scheduled.ExchangeRateSync;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.ScheduledApi.Jobs;

public class ExchangeRateSyncJob
{
    private readonly IMediator _mediator;
    private readonly IDbConnectionFactory _db;
    private readonly ILogger<ExchangeRateSyncJob> _logger;

    public ExchangeRateSyncJob(IMediator mediator, IDbConnectionFactory db, ILogger<ExchangeRateSyncJob> logger)
    {
        _mediator = mediator;
        _db = db;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Exchange rate sync started");

        var updated = await _mediator.Send(new SyncExchangeRatesCommand(), cancellationToken);

        using var connection = _db.CreateConnection();
        await connection.ExecuteAsync(
            "UPDATE job_schedule SET last_executed_at = NOW() WHERE job_key = 'exchange-rate-sync'");

        _logger.LogInformation("Exchange rate sync completed — {Count} rates updated", updated);
    }
}
