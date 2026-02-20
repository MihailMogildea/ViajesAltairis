using Dapper;
using Hangfire;
using MySqlConnector;
using ViajesAltairis.ScheduledApi.Jobs;

namespace ViajesAltairis.ScheduledApi.Services;

public static class HangfireScheduleLoader
{
    public static void LoadSchedulesFromDb(string connectionString)
    {
        using var connection = new MySqlConnection(connectionString);
        var schedules = connection.Query<(string JobKey, string CronExpression, bool Enabled)>(
            "SELECT job_key AS JobKey, cron_expression AS CronExpression, enabled AS Enabled FROM job_schedule");

        foreach (var schedule in schedules)
        {
            if (!schedule.Enabled)
            {
                RecurringJob.RemoveIfExists(schedule.JobKey);
                continue;
            }

            switch (schedule.JobKey)
            {
                case "exchange-rate-sync":
                    RecurringJob.AddOrUpdate<ExchangeRateSyncJob>(
                        schedule.JobKey, job => job.ExecuteAsync(CancellationToken.None), schedule.CronExpression);
                    break;
                case "subscription-billing":
                    RecurringJob.AddOrUpdate<SubscriptionBillingJob>(
                        schedule.JobKey, job => job.ExecuteAsync(CancellationToken.None), schedule.CronExpression);
                    break;
                case "provider-update":
                    RecurringJob.AddOrUpdate<ProviderUpdateJob>(
                        schedule.JobKey, job => job.ExecuteAsync(CancellationToken.None), schedule.CronExpression);
                    break;
            }
        }
    }
}
