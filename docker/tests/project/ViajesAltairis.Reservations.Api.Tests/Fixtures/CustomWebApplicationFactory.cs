using System.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Data.Context;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Reservations.Api.Tests.Helpers;

namespace ViajesAltairis.Reservations.Api.Tests.Fixtures;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    static CustomWebApplicationFactory()
    {
        // Dapper doesn't natively support DateOnly/TimeOnly — register type handlers
        Dapper.SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
    }

    // Repository mocks
    public IReservationRepository ReservationRepository { get; } = Substitute.For<IReservationRepository>();
    public IUserRepository UserRepository { get; } = Substitute.For<IUserRepository>();
    public IHotelRepository HotelRepository { get; } = Substitute.For<IHotelRepository>();
    public IUnitOfWork UnitOfWork { get; } = Substitute.For<IUnitOfWork>();

    // Generic repository mocks (used directly by handlers)
    public IRepository<User> UserGenericRepository { get; } = Substitute.For<IRepository<User>>();
    public IRepository<PaymentTransaction> PaymentTransactionRepository { get; } = Substitute.For<IRepository<PaymentTransaction>>();
    public IRepository<Cancellation> CancellationRepository { get; } = Substitute.For<IRepository<Cancellation>>();

    // External service mocks
    public IPaymentService PaymentService { get; } = Substitute.For<IPaymentService>();
    public IProviderReservationService ProviderReservationService { get; } = Substitute.For<IProviderReservationService>();
    public ICurrencyConverter CurrencyConverter { get; } = Substitute.For<ICurrencyConverter>();
    public ICacheService CacheService { get; } = Substitute.For<ICacheService>();
    public IEmailService EmailService { get; } = Substitute.For<IEmailService>();
    public IAuditService AuditService { get; } = Substitute.For<IAuditService>();

    // Dapper mock
    public IDbConnectionFactory ConnectionFactory { get; } = Substitute.For<IDbConnectionFactory>();

    /// <summary>
    /// Current DapperMockHelper — set this before making HTTP calls to configure Dapper responses.
    /// </summary>
    public DapperMockHelper? DapperHelper { get; set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove real DbContext registration (prevents MariaDB connection attempts)
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (dbContextDescriptor != null) services.Remove(dbContextDescriptor);

            var dbContextServiceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(AppDbContext));
            if (dbContextServiceDescriptor != null) services.Remove(dbContextServiceDescriptor);

            // Remove Redis IConnectionMultiplexer
            var redisDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IConnectionMultiplexer));
            if (redisDescriptor != null) services.Remove(redisDescriptor);

            // Replace repositories
            services.Replace(ServiceDescriptor.Scoped(_ => ReservationRepository));
            services.Replace(ServiceDescriptor.Scoped<IUserRepository>(_ => UserRepository));
            services.Replace(ServiceDescriptor.Scoped<IHotelRepository>(_ => HotelRepository));
            services.Replace(ServiceDescriptor.Scoped(_ => UnitOfWork));

            // Replace generic repositories used by handlers
            services.Replace(ServiceDescriptor.Scoped(_ => PaymentTransactionRepository));
            services.Replace(ServiceDescriptor.Scoped(_ => CancellationRepository));

            // Remove open generic IRepository<> registration and replace with specific mocks
            services.RemoveAll(typeof(IRepository<>));
            services.AddScoped(_ => UserGenericRepository);
            services.AddScoped(_ => PaymentTransactionRepository);
            services.AddScoped(_ => CancellationRepository);

            // Replace IDbConnectionFactory — wire up DapperHelper if set
            services.Replace(ServiceDescriptor.Singleton<IDbConnectionFactory>(_ =>
            {
                var factory = ConnectionFactory;
                return factory;
            }));

            // Replace external services
            services.Replace(ServiceDescriptor.Scoped(_ => PaymentService));
            services.Replace(ServiceDescriptor.Scoped(_ => ProviderReservationService));
            services.Replace(ServiceDescriptor.Scoped(_ => CurrencyConverter));
            services.Replace(ServiceDescriptor.Scoped(_ => CacheService));
            services.Replace(ServiceDescriptor.Scoped(_ => EmailService));
            services.Replace(ServiceDescriptor.Scoped(_ => AuditService));

            // Replace services that need connection strings / external dependencies
            services.Replace(ServiceDescriptor.Scoped<IEncryptionService>(_ => Substitute.For<IEncryptionService>()));
            services.Replace(ServiceDescriptor.Scoped<IPasswordService>(_ => Substitute.For<IPasswordService>()));
            services.Replace(ServiceDescriptor.Scoped<IJwtTokenService>(_ => Substitute.For<IJwtTokenService>()));
            services.Replace(ServiceDescriptor.Scoped<IReservationApiClient>(_ => Substitute.For<IReservationApiClient>()));
            services.Replace(ServiceDescriptor.Scoped<IScheduledApiClient>(_ => Substitute.For<IScheduledApiClient>()));
            services.Replace(ServiceDescriptor.Singleton<IEcbRateParser>(_ => Substitute.For<IEcbRateParser>()));
            services.Replace(ServiceDescriptor.Scoped<ITranslationService>(_ => Substitute.For<ITranslationService>()));
            services.Replace(ServiceDescriptor.Scoped<IInvoicePdfGenerator>(_ => Substitute.For<IInvoicePdfGenerator>()));
        });
    }

    /// <summary>
    /// Configure the mock IDbConnectionFactory to use a DapperMockHelper's FakeDbConnection.
    /// Call this before each test that uses Dapper queries.
    /// </summary>
    public void SetupDapperConnection(DapperMockHelper helper)
    {
        DapperHelper = helper;
        var fakeConnection = helper.BuildConnection();
        ConnectionFactory.CreateConnection().Returns(fakeConnection);
    }

    /// <summary>
    /// Reset all mock received calls between tests.
    /// </summary>
    public void ResetMocks()
    {
        ReservationRepository.ClearReceivedCalls();
        UserRepository.ClearReceivedCalls();
        UnitOfWork.ClearReceivedCalls();
        PaymentService.ClearReceivedCalls();
        ProviderReservationService.ClearReceivedCalls();
        CurrencyConverter.ClearReceivedCalls();
        PaymentTransactionRepository.ClearReceivedCalls();
        CancellationRepository.ClearReceivedCalls();
        ConnectionFactory.ClearReceivedCalls();
    }
}
