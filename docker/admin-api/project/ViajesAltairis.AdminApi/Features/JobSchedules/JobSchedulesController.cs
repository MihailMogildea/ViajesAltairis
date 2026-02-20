using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.JobSchedules.Commands;
using ViajesAltairis.Application.Features.Admin.JobSchedules.Dtos;
using ViajesAltairis.Application.Features.Admin.JobSchedules.Queries;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.AdminApi.Features.JobSchedules;

[ApiController]
[Route("api/job-schedules")]
public class JobSchedulesController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IScheduledApiClient _scheduledApiClient;

    public JobSchedulesController(ISender sender, IScheduledApiClient scheduledApiClient)
    {
        _sender = sender;
        _scheduledApiClient = scheduledApiClient;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetJobSchedulesQuery()));

    [HttpGet("{key}")]
    public async Task<IActionResult> GetByKey(string key)
    {
        var result = await _sender.Send(new GetJobScheduleByKeyQuery(key));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("{key}")]
    public async Task<IActionResult> Update(string key, UpdateJobScheduleRequest request)
    {
        var result = await _sender.Send(new UpdateJobScheduleCommand(key, request.CronExpression, request.Enabled));
        await _scheduledApiClient.ReloadSchedulesAsync();
        return Ok(result);
    }

    [HttpPost("{key}/trigger")]
    public async Task<IActionResult> Trigger(string key)
    {
        // Verify the job exists
        var schedule = await _sender.Send(new GetJobScheduleByKeyQuery(key));
        if (schedule is null)
            return NotFound();

        await _scheduledApiClient.TriggerJobAsync(key);
        return Ok(new { message = $"Job '{key}' triggered" });
    }
}
