using ES.Core.Aggregates;
using MassTransit;

namespace ES.Core.MassTransit;

public sealed class EventSourcingEndpointNameFormatter() : KebabCaseEndpointNameFormatter(true)
{

    public override string Consumer<T>()
    {
        if (typeof(T).ClosesType(typeof(AggregateCommandConsumer<,,>), out var arguments))
        {
            // Group consumer queues by aggregate type
            return GetConsumerName(arguments[0]);
        }
        return base.Consumer<T>();
    }
}
