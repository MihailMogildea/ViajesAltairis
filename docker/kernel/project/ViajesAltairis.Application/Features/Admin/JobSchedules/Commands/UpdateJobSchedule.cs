using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.JobSchedules.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.JobSchedules.Commands;

public record UpdateJobScheduleCommand(string JobKey, string CronExpression, bool Enabled) : IRequest<JobScheduleDto>;

public class UpdateJobScheduleHandler : IRequestHandler<UpdateJobScheduleCommand, JobScheduleDto>
{
    private readonly IDbConnectionFactory _db;
    public UpdateJobScheduleHandler(IDbConnectionFactory db) => _db = db;

    public async Task<JobScheduleDto> Handle(UpdateJobScheduleCommand request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();

        var rowsAffected = await connection.ExecuteAsync(
            @"UPDATE job_schedule SET cron_expression = @CronExpression, enabled = @Enabled
              WHERE job_key = @JobKey",
            new { request.CronExpression, request.Enabled, request.JobKey });

        if (rowsAffected == 0)
            throw new KeyNotFoundException($"Job schedule '{request.JobKey}' not found.");

        return (await connection.QuerySingleAsync<JobScheduleDto>(
            @"SELECT id AS Id, job_key AS JobKey, name AS Name, cron_expression AS CronExpression,
                     enabled AS Enabled, last_executed_at AS LastExecutedAt,
                     created_at AS CreatedAt, updated_at AS UpdatedAt
              FROM job_schedule WHERE job_key = @JobKey",
            new { request.JobKey }));
    }
}
