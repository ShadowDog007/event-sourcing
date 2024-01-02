using ES.Core.Aggregates.Validation;
using ES.Domain.Sample.Aggregates;
using ES.Domain.Sample.Commands;
using FluentValidation;

namespace ES.Domain.Sample.Validators;

public class SubtractValidFromSampleValidator : AggregateValidatorBase<SampleAggregate, SubtractValueFromSample>
{
    public SubtractValidFromSampleValidator()
    {
        RuleFor(x => x.Command.Value)
            .GreaterThan(0);

        RuleFor(x => x.Aggregate.State!.Value)
            .GreaterThanOrEqualTo(x => x.Command.Value);
    }
}
