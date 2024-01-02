using Microsoft.Extensions.DependencyInjection;

namespace ES.Core.Aggregates;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventSourcingAggregates(this IServiceCollection services)
    {
        services.AddScoped(typeof(IAggregateRepository<,>), typeof(AggregateRepository<,>));

        return services;
    }
}
