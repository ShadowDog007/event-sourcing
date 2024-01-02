using ES.Core.Messages.Events;
using ES.Core.Models;
using Marten;
using Marten.Services;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace ES.Core.Marten;

/// <summary>
/// Constructs SnapshotChangedEvent's from updates to async projections
/// </summary>
/// <param name="serviceProvider">Current service provider</param>
/// <param name="bus">Bus to publish snapshot changed events to</param>
public class SnapshotChangedEventPublisher(IServiceProvider serviceProvider, IBus bus) : IChangeListener
{
    private static ConcurrentDictionary<Type, ObjectFactory> _snapshotChangedEventFactories = new();

    public async Task AfterCommitAsync(IDocumentSession session, IChangeSet commit, CancellationToken token)
    {
        var updates = commit.Inserted.Concat(commit.Updated)
            .OfType<VersionedModel>()
            .Select(CreateSnapshotChanged);

        var deletes = commit.Deleted
            .Select(d => d.Document)
            .OfType<VersionedModel>()
            .Select(m => CreateSnapshotChanged(m) with { Deleted = true });

        IEnumerable<object> events =
            [
                .. updates,
                .. deletes,
            ];

        // TODO - Include this publish in transaction scope
        // TODO - Check there aren't duplicates
        await bus.PublishBatch(events, token);
    }

    public SnapshotChangedEvent CreateSnapshotChanged(VersionedModel model)
    {
        var factory = _snapshotChangedEventFactories.GetOrAdd(model.GetType(),
            t => ActivatorUtilities.CreateFactory(typeof(SnapshotChangedEvent<>).MakeGenericType(t), [t]));

        return (SnapshotChangedEvent)factory(serviceProvider, [model]);
    }

}
