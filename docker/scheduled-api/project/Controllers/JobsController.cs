using Hangfire;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.ScheduledApi.Jobs;
using ViajesAltairis.ScheduledApi.Services;

namespace ViajesAltairis.ScheduledApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public JobsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("{jobKey}/trigger")]
    public IActionResult Trigger(string jobKey)
    {
        switch (jobKey)
        {
            case "exchange-rate-sync":
                BackgroundJob.Enqueue<ExchangeRateSyncJob>(job => job.ExecuteAsync(CancellationToken.None));
                break;
            case "subscription-billing":
                BackgroundJob.Enqueue<SubscriptionBillingJob>(job => job.ExecuteAsync(CancellationToken.None));
                break;
            case "provider-update":
                BackgroundJob.Enqueue<ProviderUpdateJob>(job => job.ExecuteAsync(CancellationToken.None));
                break;
            default:
                return NotFound(new { message = $"Unknown job key: {jobKey}" });
        }

        return Ok(new { message = $"Job '{jobKey}' triggered" });
    }

    [HttpPost("reload")]
    public IActionResult Reload()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection")!;
        HangfireScheduleLoader.LoadSchedulesFromDb(connectionString);
        return Ok(new { message = "Schedules reloaded" });
    }
}
