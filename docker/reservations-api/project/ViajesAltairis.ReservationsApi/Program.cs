using Prometheus;
using ViajesAltairis.Application;
using ViajesAltairis.Data;
using ViajesAltairis.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
var redisConnectionString = builder.Configuration.GetConnectionString("Redis")!;

builder.Services.AddControllers();
builder.Services.AddApplicationServices();
builder.Services.AddDataServices(connectionString);
builder.Services.AddInfrastructureServices(redisConnectionString);

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

app.Run();

// Required for WebApplicationFactory<Program> in integration tests
public partial class Program { }
