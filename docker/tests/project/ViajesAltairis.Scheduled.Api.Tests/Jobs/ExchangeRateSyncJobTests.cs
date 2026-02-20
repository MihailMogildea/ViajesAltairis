using System.Data;
using MediatR;
using Microsoft.Extensions.Logging;
using ViajesAltairis.Application.Features.Scheduled.ExchangeRateSync;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.ScheduledApi.Jobs;

namespace ViajesAltairis.Scheduled.Api.Tests.Jobs;

public class ExchangeRateSyncJobTests
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly IDbConnectionFactory _dbFactory = Substitute.For<IDbConnectionFactory>();
    private readonly IDbConnection _connection = Substitute.For<IDbConnection>();
    private readonly ILogger<ExchangeRateSyncJob> _logger = Substitute.For<ILogger<ExchangeRateSyncJob>>();

    public ExchangeRateSyncJobTests()
    {
        _dbFactory.CreateConnection().Returns(_connection);
    }

    [Fact]
    public async Task ExecuteAsync_SendsSyncCommand()
    {
        _mediator.Send(Arg.Any<SyncExchangeRatesCommand>(), Arg.Any<CancellationToken>())
            .Returns(5);

        var job = new ExchangeRateSyncJob(_mediator, _dbFactory, _logger);

        // ExecuteAsync will fail on the Dapper ExecuteAsync (static extension on mock),
        // but we can verify MediatR was called before that point
        try
        {
            await job.ExecuteAsync(CancellationToken.None);
        }
        catch
        {
            // Expected: Dapper extension on mock connection
        }

        await _mediator.Received(1).Send(Arg.Any<SyncExchangeRatesCommand>(), Arg.Any<CancellationToken>());
    }
}
