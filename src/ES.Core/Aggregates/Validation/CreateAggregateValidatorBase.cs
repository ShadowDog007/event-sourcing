using ES.Core.Messages;
using FluentValidation;

namespace ES.Core.Aggregates.Validation;

public class CreateAggregateValidatorBase<TAggregate, TCommand> : AggregateValidatorBase<TAggregate, TCommand>
    where TAggregate : IAggregate
    where TCommand : Command
{
    public CreateAggregateValidatorBase()
    {
        RuleFor(x => x.Aggregate.Exists)
            .Equal(false)
            .WithMessage(ctx => $"{typeof(TAggregate).Name}({ctx.Command.StreamId}) already exists.");
    }
}
