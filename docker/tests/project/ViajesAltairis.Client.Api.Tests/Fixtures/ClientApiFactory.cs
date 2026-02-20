using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using StackExchange.Redis;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Client.Api.Tests.Fixtures;

public class ClientApiFactory : WebApplicationFactory<Program>
{
    public IMediator MockMediator { get; } = Substitute.For<IMediator>();
    public IReservationApiClient MockReservationApi { get; } = Substitute.For<IReservationApiClient>();
    public ICacheService MockCacheService { get; } = Substitute.For<ICacheService>();
    public IEmailService MockEmailService { get; } = Substitute.For<IEmailService>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseSetting("ConnectionStrings:DefaultConnection", "Server=localhost;Database=test;User=root;Password=test;");
        builder.UseSetting("ConnectionStrings:Redis", "localhost:6379");
        builder.UseSetting("Jwt:SecretKey", "TestSecretKeyForJwtTokenGeneration1234567890!");
        builder.UseSetting("Jwt:Issuer", "ViajesAltairis.Test");
        builder.UseSetting("Jwt:Audience", "ViajesAltairis.Test");
        builder.UseSetting("Jwt:ExpirationInMinutes", "60");
        builder.UseSetting("ReservationsApi:BaseUrl", "http://localhost:9999");

        builder.ConfigureServices(services =>
        {
            // Remove Redis connection (prevents actual connection attempt)
            services.RemoveAll<IConnectionMultiplexer>();

            // Replace services with mocks
            services.RemoveAll<ICacheService>();
            services.AddScoped(_ => MockCacheService);

            services.RemoveAll<IEmailService>();
            services.AddScoped(_ => MockEmailService);

            services.RemoveAll<IReservationApiClient>();
            services.AddScoped(_ => MockReservationApi);

            // Replace MediatR so controller tests bypass handlers
            services.RemoveAll<IMediator>();
            services.RemoveAll<ISender>();
            services.AddScoped<IMediator>(_ => MockMediator);
            services.AddScoped<ISender>(_ => MockMediator);
        });
    }
}
