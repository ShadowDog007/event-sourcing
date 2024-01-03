using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ES.Core.Health;

public static class EndpointRouteBuilderExtensions
{
    public static void MapEventSourcingHealthChecks(this IEndpointRouteBuilder routes)
    {
        routes.MapHealthChecks("/health/startup", new HealthCheckOptions
        {
            Predicate = IncludesTags("startup", "masstransit"),
            ResponseWriter = HealthCheckResponseWriter,
        });
        routes.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = IncludesTags("ready"),
            ResponseWriter = HealthCheckResponseWriter,
        });
        routes.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = HealthCheckResponseWriter,
        });
    }

    private static Func<HealthCheckRegistration, bool> IncludesTags(params string[] tags)
    {
        return registration => tags.Any(registration.Tags.Contains);
    }

    private static async Task HealthCheckResponseWriter(HttpContext context, HealthReport report)
    {
        await context.Response.WriteAsJsonAsync(report, context.RequestAborted);
    }

}
