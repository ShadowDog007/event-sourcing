using ES.Core.Aggregates.Validation;
using ES.Domain.Sample.Aggregates;
using ES.Domain.Sample.Commands;
using FluentValidation;

namespace ES.Domain.Sample.Validators;

public class AddValueToSampleValidator : AggregateValidatorBase<SampleAggregate, AddValueToSample>
{
    public AddValueToSampleValidator()
    {
        RuleFor(x => x.Command.Value)
            .GreaterThan(0);
    }
}
