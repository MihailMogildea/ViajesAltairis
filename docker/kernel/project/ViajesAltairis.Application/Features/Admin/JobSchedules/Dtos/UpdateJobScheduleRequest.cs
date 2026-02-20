namespace ViajesAltairis.Application.Features.Admin.JobSchedules.Dtos;

public record UpdateJobScheduleRequest(string CronExpression, bool Enabled);
