using Marten;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ES.Core.Health;

/// <summary>
/// Performs a check to see if the database is accessable
/// </summary>
/// <param name="store">Document store to use to create the connection</param>
public class DatabaseHealthCheck(IDocumentStore store) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var session = store.QuerySession();
            await session.QueryAsync<int>("select 1");
            return HealthCheckResult.Healthy($"Connection to database successful");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Connection to database failed", ex);
        }
    }
}
