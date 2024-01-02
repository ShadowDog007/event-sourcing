using ES.Core.Messages;
using FluentValidation;

namespace ES.Core.Aggregates.Validation;

public class AggregateValidatorBase<TAggregate, TCommand>
    : AbstractValidator<ValidationContext<TAggregate, TCommand>>,
        IAggregateValidator<TAggregate, TCommand>
    
    where TAggregate : IAggregate
    where TCommand : Command
{
    public AggregateValidatorBase()
    {
        if (!typeof(TCommand).ClosesType(typeof(ICommandSupportsStart), out _))
        {
            RuleFor(x => x.Aggregate.Exists)
                .Equal(true);
        }
    }
}
