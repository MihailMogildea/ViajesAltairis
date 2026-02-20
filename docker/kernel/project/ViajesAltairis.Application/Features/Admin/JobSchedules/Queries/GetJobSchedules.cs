using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.JobSchedules.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.JobSchedules.Queries;

public record GetJobSchedulesQuery : IRequest<IEnumerable<JobScheduleDto>>;

public class GetJobSchedulesHandler : IRequestHandler<GetJobSchedulesQuery, IEnumerable<JobScheduleDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetJobSchedulesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<JobScheduleDto>> Handle(GetJobSchedulesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<JobScheduleDto>(
            @"SELECT id AS Id, job_key AS JobKey, name AS Name, cron_expression AS CronExpression,
                     enabled AS Enabled, last_executed_at AS LastExecutedAt,
                     created_at AS CreatedAt, updated_at AS UpdatedAt
              FROM job_schedule ORDER BY name");
    }
}
