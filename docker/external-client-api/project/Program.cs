using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ViajesAltairis.Application;
using ViajesAltairis.Data;
using ViajesAltairis.Infrastructure;
using Prometheus;
using ViajesAltairis.ExternalClientApi.Authentication;
using ViajesAltairis.ExternalClientApi.Configuration;
using ViajesAltairis.ExternalClientApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// Kernel services
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
builder.Services.AddApplicationServices();
builder.Services.AddDataServices(connectionString);
builder.Services.AddInfrastructureServices(redisConnectionString);

// HttpClient for reservations-api proxy
builder.Services.AddHttpClient("ReservationsApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ReservationsApi:BaseUrl"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// JWT Token Service
builder.Services.AddSingleton<JwtTokenService>();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ViajesAltairis External Client API",
        Version = "v1",
        Description = "B2B partner API for hotel search and reservations"
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

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseHttpMetrics();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<BusinessPartnerAuthorizationMiddleware>();
app.MapControllers();
app.MapMetrics();

app.Run();
