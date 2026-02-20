using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ViajesAltairis.Application;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.ClientApi.Middleware;
using ViajesAltairis.ClientApi.Services;
using Prometheus;
using ViajesAltairis.Data;
using ViajesAltairis.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddApplicationServices();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection not configured.");
builder.Services.AddDataServices(connectionString);

var redisConnectionString = builder.Configuration.GetConnectionString("Redis")
    ?? throw new InvalidOperationException("Redis connection string not configured.");
builder.Services.AddInfrastructureServices(redisConnectionString);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddHttpClient("ReservationsApi", client =>
{
    var baseUrl = builder.Configuration["ReservationsApi:BaseUrl"] ?? "http://reservations-api:8080";
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddHttpClient("ProvidersApi", client =>
{
    var baseUrl = builder.Configuration["ProvidersApi:BaseUrl"] ?? "http://providers-api:8080";
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtConfig = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig["Issuer"],
            ValidAudience = jwtConfig["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtConfig["SecretKey"]!)),
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ViajesAltairis Client API",
        Version = "v1",
        Description = "Client-facing API for hotel reservations"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3002")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseHttpMetrics();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapMetrics();

app.Run();

public partial class Program { }
