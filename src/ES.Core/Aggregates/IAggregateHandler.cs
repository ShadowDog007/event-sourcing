using ES.Core.Messages;

namespace ES.Core.Aggregates;

public interface IAggregateHandler<TAggregate>
{
}

public interface IAggregateHandler<TAggregate, TCommand> : IAggregateHandler<TAggregate>
    where TAggregate : IAggregate, IAggregateHandles<TCommand>
    where TCommand : Command
{
    ValueTask HandleAsync(TAggregate aggregate, TCommand command, CancellationToken cancellationToken);
}
