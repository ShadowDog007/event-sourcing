using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ES.Core.Telemetry;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventSourcingTelemetry(this IServiceCollection services, IWebHostEnvironment environment)
    {
        services.AddOpenTelemetry()
            .WithTracing(b => b.ConfigureTracing(environment))
            .WithMetrics(b => b.ConfigureMetrics(environment));

        return services;
    }

    private static void ConfigureTracing(this TracerProviderBuilder builder, IWebHostEnvironment environment)
    {
        builder.ConfigureResource(r => r
            .AddService(environment.ApplicationName, environment.EnvironmentName)
            .AddEnvironmentVariableDetector());

        builder.SetSampler(new ParentBasedSampler(new AlwaysOnSampler()));

        builder
            .AddAspNetCoreInstrumentation()
            .AddMassTransitInstrumentation()
            .AddNpgsql()
            .AddHttpClientInstrumentation();

        builder.AddOtlpExporter();
    }

    private static void ConfigureMetrics(this MeterProviderBuilder builder, IWebHostEnvironment environment)
    {
        builder.ConfigureResource(r => r
            .AddService(environment.ApplicationName, environment.EnvironmentName)
            .AddEnvironmentVariableDetector());

        builder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddMeter(global::MassTransit.Monitoring.InstrumentationOptions.MeterName)
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation();

        builder.AddOtlpExporter();
    }
}
