using ES.Core.Aggregates;
using ES.Domain.Sample.Commands;
using ES.Domain.Sample.Events;
using ES.Domain.Sample.Models;
using ES.Domain.Sample.Services;

namespace ES.Domain.Sample.Aggregates;

public partial class SampleAggregate : AggregateBase<SampleModel>
{
    public void Handle(AddValueToSample command)
        => Append(new SampleValueAdded(command));

    public void Handle(SubtractValueFromSample command, ISampleService service1, ISampleService service2)
        => Append(new SampleValueSubtracted(command));
}
