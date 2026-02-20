using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Data.Context;

namespace ViajesAltairis.Admin.Api.Tests.Infrastructure;

public class AdminApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private SqliteConnection _keepAliveConnection = null!;
    private string _connectionString = null!;

    public Task InitializeAsync()
    {
        // Register Dapper type handlers for SQLite compatibility
        Dapper.SqlMapper.AddTypeHandler(new SqliteDateTimeHandler());
        Dapper.SqlMapper.AddTypeHandler(new SqliteTimeOnlyHandler());
        Dapper.SqlMapper.AddTypeHandler(new SqliteBoolHandler());
        Dapper.SqlMapper.AddTypeHandler(new SqliteDateOnlyHandler());
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        // Shared in-memory SQLite DB — keep-alive connection prevents it from being destroyed
        var dbName = $"testdb_{Guid.NewGuid():N}";
        _connectionString = $"Data Source={dbName};Mode=Memory;Cache=Shared";
        _keepAliveConnection = new SqliteConnection(_connectionString);
        _keepAliveConnection.Open();

        // Create schema via EF Core
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();

        // Create the 'user' table for Dapper auth queries (EF maps to different table name conventions)
        // EnsureCreated uses EF configurations which map tables with snake_case.
        // We also need to seed a test admin user for auth tests.
        SeedTestData(scope.ServiceProvider);

        return Task.CompletedTask;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("DetailedErrors", "true");

        // Inject JWT settings via configuration so Program.cs picks them up
        // before creating the SymmetricSecurityKey
        builder.UseSetting("Jwt:SecretKey", TestAuthHelper.TestSecretKey);
        builder.UseSetting("Jwt:Issuer", TestAuthHelper.TestIssuer);
        builder.UseSetting("Jwt:Audience", TestAuthHelper.TestAudience);
        builder.UseSetting("Jwt:ExpirationHours", "8");

        // Store last exception for diagnostics
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.SetMinimumLevel(LogLevel.Warning);
        });

        builder.ConfigureServices(services =>
        {
            // Remove ALL DbContext-related registrations including internal
            // IDbContextOptionsConfiguration<AppDbContext> callbacks that would
            // trigger ServerVersion.AutoDetect → MySQL connection attempt
            var dbDescriptors = services.Where(d =>
                d.ServiceType == typeof(AppDbContext) ||
                d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                (d.ServiceType.IsGenericType &&
                 d.ServiceType.GenericTypeArguments.Length == 1 &&
                 d.ServiceType.GenericTypeArguments[0] == typeof(AppDbContext)))
                .ToList();
            foreach (var d in dbDescriptors)
                services.Remove(d);

            // Register SQLite-backed AppDbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(_connectionString)
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging());

            // Replace Dapper connection factory with SQLite
            services.RemoveAll<IDbConnectionFactory>();
            services.AddSingleton<IDbConnectionFactory>(new SqliteDbConnectionFactory(_connectionString));

            // Replace cache with in-memory fake
            services.RemoveAll<ICacheService>();
            services.AddSingleton<ICacheService, FakeCacheService>();

            // Replace password service with fake
            services.RemoveAll<IPasswordService>();
            services.AddSingleton<IPasswordService, FakePasswordService>();

            // Replace external API clients with mocks
            services.RemoveAll<IReservationApiClient>();
            services.AddSingleton(NSubstitute.Substitute.For<IReservationApiClient>());

            services.RemoveAll<IScheduledApiClient>();
            services.AddSingleton(NSubstitute.Substitute.For<IScheduledApiClient>());

            // Replace encryption service with fake (AES needs a real key)
            services.RemoveAll<IEncryptionService>();
            services.AddSingleton<IEncryptionService, FakeEncryptionService>();

            // Replace currency converter with fake
            services.RemoveAll<ICurrencyConverter>();
            services.AddSingleton<ICurrencyConverter, FakeCurrencyConverter>();

            // Configure JWT with known test values
            services.Configure<ViajesAltairis.AdminApi.Auth.JwtSettings>(opts =>
            {
                opts.SecretKey = TestAuthHelper.TestSecretKey;
                opts.Issuer = TestAuthHelper.TestIssuer;
                opts.Audience = TestAuthHelper.TestAudience;
                opts.ExpirationHours = 8;
            });

            // JWT bearer options are configured via UseSetting("Jwt:*") above
        });
    }

    private void SeedTestData(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();

        // Seed a user type for admin (id=1)
        db.Database.ExecuteSqlRaw(
            "INSERT OR IGNORE INTO user_type (id, name, created_at) VALUES (1, 'Admin', datetime('now'))");
        db.Database.ExecuteSqlRaw(
            "INSERT OR IGNORE INTO user_type (id, name, created_at) VALUES (2, 'Manager', datetime('now'))");
        db.Database.ExecuteSqlRaw(
            "INSERT OR IGNORE INTO user_type (id, name, created_at) VALUES (3, 'Agent', datetime('now'))");
        db.Database.ExecuteSqlRaw(
            "INSERT OR IGNORE INTO user_type (id, name, created_at) VALUES (4, 'Hotel Staff', datetime('now'))");
        db.Database.ExecuteSqlRaw(
            "INSERT OR IGNORE INTO user_type (id, name, created_at) VALUES (5, 'Client', datetime('now'))");

        // Seed a test admin user for login tests
        db.Database.ExecuteSqlRaw(
            """
            INSERT OR IGNORE INTO user (id, email, password_hash, first_name, last_name, user_type_id, enabled, created_at)
            VALUES (1, 'admin@test.com', 'HASHED:password123', 'Test', 'Admin', 1, 1, datetime('now'))
            """);

        // Seed a disabled user
        db.Database.ExecuteSqlRaw(
            """
            INSERT OR IGNORE INTO user (id, email, password_hash, first_name, last_name, user_type_id, enabled, created_at)
            VALUES (2, 'disabled@test.com', 'HASHED:password123', 'Disabled', 'User', 1, 0, datetime('now'))
            """);

        // Seed a client user (should not be able to log into admin panel)
        db.Database.ExecuteSqlRaw(
            """
            INSERT OR IGNORE INTO user (id, email, password_hash, first_name, last_name, user_type_id, enabled, created_at)
            VALUES (3, 'client@test.com', 'HASHED:password123', 'Client', 'User', 5, 1, datetime('now'))
            """);
    }

    public new async Task DisposeAsync()
    {
        if (_keepAliveConnection is not null)
        {
            await _keepAliveConnection.CloseAsync();
            await _keepAliveConnection.DisposeAsync();
        }
        await base.DisposeAsync();
    }
}

public class FakeEncryptionService : IEncryptionService
{
    public string Encrypt(string plainText) => "ENC:" + plainText;
    public string Decrypt(string cipherText) => cipherText.StartsWith("ENC:") ? cipherText[4..] : cipherText;
}

public class FakeCurrencyConverter : ICurrencyConverter
{
    public Task<(decimal convertedAmount, long exchangeRateId)> ConvertAsync(
        decimal amount, long sourceCurrencyId, long targetCurrencyId, CancellationToken ct = default)
    {
        return Task.FromResult((amount, 1L));
    }
}
