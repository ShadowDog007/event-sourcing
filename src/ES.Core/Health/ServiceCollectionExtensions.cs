using Google.Protobuf.WellKnownTypes;
using Marten.Services;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
