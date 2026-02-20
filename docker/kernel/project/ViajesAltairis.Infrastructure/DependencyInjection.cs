using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Infrastructure.Audit;
using ViajesAltairis.Infrastructure.Cache;
using ViajesAltairis.Infrastructure.Email;
using ViajesAltairis.Infrastructure.Payment;
using ViajesAltairis.Infrastructure.Providers;
using ViajesAltairis.Infrastructure.Security;
using ViajesAltairis.Infrastructure.Currency;
using ViajesAltairis.Infrastructure.Services;
using ViajesAltairis.Infrastructure.Translations;

namespace ViajesAltairis.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string redisConnectionString)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(redisConnectionString));
        services.AddScoped<ICacheService, Cache.RedisCacheService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IReservationApiClient, ReservationApiClient>();
        services.AddScoped<IPaymentService, Payment.PaymentService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddHttpClient<IProviderReservationService, ProviderReservationApiClient>(client =>
        {
            client.BaseAddress = new Uri("http://providers-api:8080/");
        });

        services.AddScoped<IProvidersApiClient, ProvidersApiClient>();
        services.AddScoped<IScheduledApiClient, ScheduledApiClient>();
        services.AddSingleton<IEcbRateParser, EcbRateParser>();
        services.AddScoped<ITranslationService, TranslationService>();
        services.AddScoped<ICurrencyConverter, CurrencyConverter>();

        return services;
    }
}
