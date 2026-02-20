using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Data.Context;
using ViajesAltairis.Data.Repositories;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IHotelRepository, HotelRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddSingleton<IDbConnectionFactory, DapperConnectionFactory>();

        return services;
    }
}
