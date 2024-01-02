using ES.Core.Models;
using Marten.Events;

namespace ES.Core.Aggregates;

public interface IAggregateConstructor<TAggregate, TState>
    where TAggregate : IAggregate<TState>, IAggregateConstructor<TAggregate, TState>
    where TState : VersionedModel
{
    abstract static TAggregate Create(IEventStream<TState> eventStream, TState? state);
}
