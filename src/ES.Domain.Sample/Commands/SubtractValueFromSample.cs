using ES.Core.Messages;

namespace ES.Domain.Sample.Commands;

public record SubtractValueFromSample : Command
{
    public required int Value { get; init; }
}
