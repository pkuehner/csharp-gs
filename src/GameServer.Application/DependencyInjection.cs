using GameServer.Application.Users.Connect;
using Microsoft.Extensions.DependencyInjection;

namespace GameServer.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ConnectUserHandler>();
        return services;
    }
}
