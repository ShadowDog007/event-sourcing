using ES.Core.Messages;
using FluentValidation;

namespace ES.Core.Aggregates.Validation;

public class UpdateAggregateValidatorBase<TAggregate, TCommand> : AggregateValidatorBase<TAggregate, TCommand>
    where TAggregate : IAggregate
    where TCommand : Command
{
    public UpdateAggregateValidatorBase()
    {
        RuleFor(x => x.Aggregate.Exists)
            .Equal(true)
            .WithMessage(ctx => $"{typeof(TAggregate).Name}({ctx.Command.StreamId}) does not exist.");
    }
}
