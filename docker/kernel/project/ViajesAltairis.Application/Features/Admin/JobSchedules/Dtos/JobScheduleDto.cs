namespace ViajesAltairis.Application.Features.Admin.JobSchedules.Dtos;

public record JobScheduleDto(
    long Id,
    string JobKey,
    string Name,
    string CronExpression,
    bool Enabled,
    DateTime? LastExecutedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt);
