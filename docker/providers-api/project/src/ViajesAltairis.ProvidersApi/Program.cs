using Prometheus;
using StackExchange.Redis;
using ViajesAltairis.ProvidersApi.ExternalClients;
using ViajesAltairis.ProvidersApi.ExternalClients.HotelBeds;
using ViajesAltairis.ProvidersApi.ExternalClients.BookingDotCom;
using ViajesAltairis.ProvidersApi.ExternalClients.TravelGate;
using ViajesAltairis.ProvidersApi.Repositories;
using ViajesAltairis.ProvidersApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// External provider clients
builder.Services.AddSingleton<IExternalProviderClient, HotelBedsClient>();
builder.Services.AddSingleton<IExternalProviderClient, BookingDotComClient>();
builder.Services.AddSingleton<IExternalProviderClient, TravelGateClient>();

// Repositories
builder.Services.AddSingleton<IProviderRepository, ProviderRepository>();
builder.Services.AddSingleton<IHotelSyncRepository, HotelSyncRepository>();

// Redis
var redisConnectionString = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

// Services
builder.Services.AddSingleton<SyncService>();
builder.Services.AddHostedService<ProviderRegistrationService>();

var app = builder.Build();

app.UseHttpMetrics();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.MapControllers();
app.MapMetrics();
app.MapHealthChecks("/health");

app.Run();
