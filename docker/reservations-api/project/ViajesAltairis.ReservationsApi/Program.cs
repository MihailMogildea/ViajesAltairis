using Prometheus;
using QuestPDF.Infrastructure;
using ViajesAltairis.Application;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Data;
using ViajesAltairis.Infrastructure;
using ViajesAltairis.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsEnvironment("Testing"))
    QuestPDF.Settings.License = LicenseType.Community;

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
var redisConnectionString = builder.Configuration["Redis:ConnectionString"]!;

builder.Services.AddControllers();
builder.Services.AddApplicationServices();
builder.Services.AddDataServices(connectionString);
builder.Services.AddInfrastructureServices(redisConnectionString);
builder.Services.AddScoped<IInvoicePdfGenerator, InvoicePdfGenerator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpMetrics();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapMetrics();

app.Run();

// Required for WebApplicationFactory<Program> in integration tests
public partial class Program { }
