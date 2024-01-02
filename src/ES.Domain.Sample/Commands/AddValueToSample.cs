using ES.Core.Messages;

namespace ES.Domain.Sample.Commands;

public record AddValueToSample : Command, ICommandSupportsStart
{
    public required int Value { get; init; }
}
