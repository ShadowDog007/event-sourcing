using ES.Domain.Sample.Services;

namespace ES.Domain.Sample;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// Source generators automatically discover and invoke any service collection extensions
    /// As long as the method name starts with `Add` and uses `this IServiceCollection services`
    ///     with an optional `IWebHostEnvironment environment` parameter.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="environment"></param>
    /// <returns></returns>
    public static IServiceCollection AddDomainSampleServices(this IServiceCollection services,
        IWebHostEnvironment environment)
    {
        services.AddSingleton<ISampleService, SampleService>();

        return services;
    }
}
