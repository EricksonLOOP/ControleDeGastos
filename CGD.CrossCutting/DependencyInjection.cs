using CGD.CrossCutting.Security;
using Microsoft.Extensions.DependencyInjection;

namespace CGD.CrossCutting;

public static class DependencyInjection
{
    public static IServiceCollection AddCrossCutting(this IServiceCollection services)
    {
        services.AddScoped<PasswordHash>();

        return services;
    }
}