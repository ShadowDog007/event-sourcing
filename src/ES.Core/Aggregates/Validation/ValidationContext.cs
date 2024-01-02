using ES.Core.Messages;

namespace ES.Core.Aggregates.Validation;

public record ValidationContext<TAggregate, TCommand>(TAggregate Aggregate, TCommand Command)
    where TAggregate : IAggregate
    where TCommand : Command;
