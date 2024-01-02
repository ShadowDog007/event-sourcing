using ES.Core.Models;
using Marten.Events.Aggregation;

namespace ES.Core.Projections;

public class AggregateProjection<TState> : SingleStreamProjection<TState>
    where TState : VersionedModel
{
    protected AggregateProjection() { }
}
