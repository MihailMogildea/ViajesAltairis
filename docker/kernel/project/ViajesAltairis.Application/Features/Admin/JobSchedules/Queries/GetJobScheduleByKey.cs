using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.JobSchedules.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.JobSchedules.Queries;

public record GetJobScheduleByKeyQuery(string JobKey) : IRequest<JobScheduleDto?>;

public class GetJobScheduleByKeyHandler : IRequestHandler<GetJobScheduleByKeyQuery, JobScheduleDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetJobScheduleByKeyHandler(IDbConnectionFactory db) => _db = db;

    public async Task<JobScheduleDto?> Handle(GetJobScheduleByKeyQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<JobScheduleDto>(
            @"SELECT id AS Id, job_key AS JobKey, name AS Name, cron_expression AS CronExpression,
                     enabled AS Enabled, last_executed_at AS LastExecutedAt,
                     created_at AS CreatedAt, updated_at AS UpdatedAt
              FROM job_schedule WHERE job_key = @JobKey",
            new { request.JobKey });
    }
}
