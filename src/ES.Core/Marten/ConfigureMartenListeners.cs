using Marten;
using Microsoft.Extensions.DependencyInjection;

namespace ES.Core.Marten;

public class ConfigureMartenListeners : IConfigureMarten
{
    public void Configure(IServiceProvider services, StoreOptions options)
    {
        foreach (var listener in services.GetServices<IDocumentSessionListener>())
            options.Listeners.Add(listener);
        foreach (var listener in services.GetServices<IChangeListener>())
            options.Projections.AsyncListeners.Add(listener);
    }
}
