using ES.Core.Aggregates;
using ES.Core.Marten;
using ES.Core.MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ES.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventSourcingCore(this IServiceCollection services, IWebHostEnvironment environment)
    {
        // Microsoft Dependencies
        services.AddOptions();
        services.AddLogging(b =>
        {
            if (environment.IsDevelopment())
                return;
            b.ClearProviders();
            b.AddJsonConsole(o =>
            {
                o.IncludeScopes = true;
                o.UseUtcTimestamp = true;
            });
        });
        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer");
        services.AddOptions<JwtBearerOptions>("Bearer")
            .BindConfiguration(nameof(JwtBearerOptions));

        // Swagger
        services.AddEndpointsApiExplorer()
            .AddSwaggerGen();

        // Internal dependencies
        services
            .AddEventSourcingAggregates()
            .AddEventSourcingMarten()
            .AddEventSourcingMassTransit();

        return services;
    }
}
