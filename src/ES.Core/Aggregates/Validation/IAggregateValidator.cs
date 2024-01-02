using ES.Core.Messages;
using FluentValidation;

namespace ES.Core.Aggregates.Validation;

public interface IAggregateValidator<TAggregate, TCommand> : IValidator<ValidationContext<TAggregate, TCommand>>
    where TAggregate : IAggregate
    where TCommand : Command;
