using ES.Core.Projections;
using ES.Domain.Sample.Events;
using ES.Domain.Sample.Models;

namespace ES.Domain.Sample.Projections;

public class SampleProjection : AggregateProjection<SampleModel>
{
    public SampleProjection()
    {
    }

    public SampleModel Apply(SampleModel current, SampleValueAdded @event)
        => current.Apply(@event) with { Value = current.Value + @event.Value };

    public SampleModel Apply(SampleModel current, SampleValueSubtracted @event)
        => current.Apply(@event) with { Value = current.Value - @event.Value };
}
