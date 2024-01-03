using ES.Core.Messages;

namespace ES.Domain.Sample.Commands;

/// <summary>
/// Adds <see cref="Value"/>
/// </summary>
public record AddValueToSample : Command, ICommandSupportsStart
{
    public required int Value { get; init; }
}
