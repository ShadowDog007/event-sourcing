using ES.Core.Models;
using Marten.Events;

namespace ES.Core.Aggregates;

public interface IAggregate
{
    /// <summary>
    /// Flag to show if the aggregate stream has been created yet
    /// </summary>
    bool Exists { get; }

    /// <summary>
    /// Current version of the aggregate
    /// </summary>
    long Version { get; }
}

public interface IAggregate<TState> : IAggregate
    where TState : VersionedModel
{
    /// <summary>
    /// The event stream for the aggregate
    /// </summary>
    IEventStream<TState> Stream { get; init; }

    /// <summary>
    /// The current state of the aggregate
    /// </summary>
    TState? State { get; }
}
