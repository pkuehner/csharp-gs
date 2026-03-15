using GameServer.Application.Abstractions;
using GameServer.Infrastructure.Persistence;
using GameServer.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GameServer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<GameServerDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IUserNameGenerator, RandomUserNameGenerator>();
        services.AddSingleton<IUuidGenerator, UuidGenerator>();
        services.AddSingleton<IClock, SystemClock>();

        return services;
    }
}
