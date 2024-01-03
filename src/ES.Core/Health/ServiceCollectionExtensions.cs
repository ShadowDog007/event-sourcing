using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ES.Core.Health;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds health checks
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddEventSourcingHealthChecks(this IServiceCollection services)
    {
        services.AddScoped<IHealthCheck, DatabaseHealthCheck>();

        services.AddHealthChecks()
            // Add database health check
            .AddCheck<DatabaseHealthCheck>("database", tags: ["ready", "startup"]);
        
        // Note: MassTransit add's it's own health check under ["ready", "masstransit"] tags

        return services;
    }
}
