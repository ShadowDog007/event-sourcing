using Marten;
using Marten.Events.Daemon.Resiliency;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace ES.Core.Marten;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddEventSourcingMarten(this IServiceCollection services)
    {
        services.AddTransient<IConfigureMarten, ConfigureMartenListeners>();
        services.AddSingleton<IConnectionStringProvider, NpgsqlConnectionStringProvider>();

        services.AddTransient<IDocumentSessionListener, EventPublisher>();
        services.AddTransient<IChangeListener, SnapshotChangedEventPublisher>();

        services.AddOptions<NpgsqlConnectionStringBuilder>()
            .BindConfiguration($"Marten:{nameof(NpgsqlConnectionStringBuilder)}")
            .Validate(v => !string.IsNullOrEmpty(v.Host), $"Marten:{nameof(NpgsqlConnectionStringBuilder)}:{nameof(NpgsqlConnectionStringBuilder.Host)} must be configured")
            .ValidateOnStart();

        var marten = services.AddMarten(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();

            var options = new StoreOptions();
            configuration.GetSection($"Marten:{nameof(StoreOptions)}").Bind(options);

            var connectionStringProvider = sp.GetRequiredService<IConnectionStringProvider>();
            options.Connection(() => connectionStringProvider.ConnectionString);

            return options;
        });

        marten.BuildSessionsWith<TransactionScopeSessionFactory>();

        // TODO - This only runs on one instance, to avoid the load sitting on a single instance
        //  this should be split onto a separate instance, or look into an option for sharded parallel processing
        marten.AddAsyncDaemon(DaemonMode.HotCold);

        // TODO - Best practice is to apply database changes separate to application deployment, this should be split into a separate application
        marten.ApplyAllDatabaseChangesOnStartup();

        return services;
    }
}
