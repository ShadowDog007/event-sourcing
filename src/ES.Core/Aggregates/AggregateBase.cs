using ES.Core.Messages;
using ES.Core.Models;
using Marten.Events;

namespace ES.Core.Aggregates;

/// <summary>
/// Base class for creation of aggregates
/// </summary>
/// <typeparam name="TState"></typeparam>
public abstract class AggregateBase<TState> : IAggregate<TState>
    where TState : VersionedModel
{
    public required IEventStream<TState> Stream { get; init; }

    public TState? State { get; init; }
    public bool Exists => Stream.StartingVersion > 0;

    public void Append(Event @event) => Stream.AppendOne(@event);
}
