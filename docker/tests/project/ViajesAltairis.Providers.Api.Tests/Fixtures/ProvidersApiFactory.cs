using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using ViajesAltairis.ProvidersApi.Controllers;
using ViajesAltairis.ProvidersApi.Repositories;
using ViajesAltairis.ProvidersApi.Services;

namespace ViajesAltairis.Providers.Api.Tests.Fixtures;

public class ProvidersApiFactory : WebApplicationFactory<ProvidersController>
{
    public IProviderRepository MockProviderRepo { get; } = Substitute.For<IProviderRepository>();
    public IHotelSyncRepository MockHotelSyncRepo { get; } = Substitute.For<IHotelSyncRepository>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove the hosted service so it doesn't run on startup
            var hostedServiceDescriptor = services.FirstOrDefault(
                d => d.ImplementationType == typeof(ProviderRegistrationService));
            if (hostedServiceDescriptor != null)
                services.Remove(hostedServiceDescriptor);

            // Replace real repositories with mocks
            var providerRepoDescriptor = services.FirstOrDefault(
                d => d.ServiceType == typeof(IProviderRepository));
            if (providerRepoDescriptor != null)
                services.Remove(providerRepoDescriptor);

            var hotelSyncRepoDescriptor = services.FirstOrDefault(
                d => d.ServiceType == typeof(IHotelSyncRepository));
            if (hotelSyncRepoDescriptor != null)
                services.Remove(hotelSyncRepoDescriptor);

            services.AddSingleton(MockProviderRepo);
            services.AddSingleton(MockHotelSyncRepo);

            // Provide a mock IConnectionMultiplexer so SyncService can be resolved
            services.AddSingleton(Substitute.For<IConnectionMultiplexer>());
        });
    }
}
