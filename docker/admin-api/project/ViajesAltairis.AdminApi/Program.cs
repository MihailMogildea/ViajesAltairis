using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using StackExchange.Redis;
using ViajesAltairis.AdminApi.Auth;
using ViajesAltairis.AdminApi.Middleware;
using ViajesAltairis.AdminApi.Services;
using ViajesAltairis.Application;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Data.Context;
using ViajesAltairis.Data.Repositories;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Infrastructure.Currency;
using Prometheus;
using QuestPDF.Infrastructure;
using ViajesAltairis.Infrastructure.Cache;
using ViajesAltairis.Infrastructure.Services;
using ViajesAltairis.Infrastructure.Translations;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((context, config) => config
    .ReadFrom.Configuration(context.Configuration)
    .WriteTo.Console());

// EF Core with Pomelo
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Dapper connection factory
builder.Services.AddScoped<IDbConnectionFactory, DapperConnectionFactory>();

// Unit of Work
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

// Generic repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(ISimpleRepository<>), typeof(SimpleRepository<>));

// Specialized repositories
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Application layer (MediatR, FluentValidation, pipeline behaviors)
builder.Services.AddApplicationServices();

// Redis
var redisConnectionString = builder.Configuration["Redis:ConnectionString"];
if (!string.IsNullOrEmpty(redisConnectionString))
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
    builder.Services.AddScoped<ICacheService, RedisCacheService>();
}

// Infrastructure services
builder.Services.Configure<EncryptionSettings>(builder.Configuration.GetSection("Encryption"));
builder.Services.AddScoped<IEncryptionService, AesEncryptionService>();
builder.Services.AddScoped<IPasswordService, BcryptPasswordService>();
builder.Services.AddScoped<ICurrencyConverter, CurrencyConverter>();
builder.Services.AddScoped<ITranslationService, TranslationService>();
builder.Services.AddScoped<IInvoicePdfGenerator, InvoicePdfGenerator>();

// JWT Authentication
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        };
    });
builder.Services.AddAuthorization();

// Scheduled API client
builder.Services.AddHttpClient("ScheduledApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ScheduledApi:BaseUrl"] ?? "http://scheduled-api:8080/");
});
builder.Services.AddScoped<IScheduledApiClient, ScheduledApiClient>();

// Reservations API client
builder.Services.AddHttpClient("ReservationsApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ReservationsApi:BaseUrl"] ?? "http://reservations-api:8080/");
});
builder.Services.AddScoped<IReservationApiClient, ReservationApiClient>();

// Payment & provider services (needed for ConfirmBankTransfer flow)
builder.Services.AddScoped<IPaymentService, ViajesAltairis.Infrastructure.Payment.PaymentService>();
builder.Services.AddHttpClient<IProviderReservationService, ViajesAltairis.Infrastructure.Providers.ProviderReservationApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ProvidersApi:BaseUrl"] ?? "http://providers-api:8080/");
});

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.FullName);
});

var app = builder.Build();

app.UseHttpMetrics();

// Middleware
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RoleAuthorizationMiddleware>();

app.MapControllers();
app.MapMetrics();

app.Run();

public partial class Program { }
