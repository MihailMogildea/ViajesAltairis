using Hangfire;
using Prometheus;
using Hangfire.MySql;
using ViajesAltairis.Application;
using ViajesAltairis.Data;
using ViajesAltairis.Infrastructure;
using ViajesAltairis.ScheduledApi.Jobs;
using ViajesAltairis.ScheduledApi.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
var redisConnectionString = builder.Configuration.GetConnectionString("Redis")!;

builder.Services.AddControllers();
builder.Services.AddApplicationServices();
builder.Services.AddDataServices(connectionString);
builder.Services.AddInfrastructureServices(redisConnectionString);

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseStorage(new MySqlStorage(connectionString, new MySqlStorageOptions
    {
        TablesPrefix = "hangfire_"
    })));

builder.Services.AddHangfireServer();
builder.Services.AddHttpClient();

builder.Services.AddScoped<ExchangeRateSyncJob>();
builder.Services.AddScoped<SubscriptionBillingJob>();
builder.Services.AddScoped<ProviderUpdateJob>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpMetrics();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapMetrics();
app.UseHangfireDashboard("/hangfire");

HangfireScheduleLoader.LoadSchedulesFromDb(connectionString);

app.Run();
