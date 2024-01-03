using ES.Core.Messages;

namespace ES.Domain.Sample.Commands;

/// <summary>
/// Subtracts <see cref="Value"/>
/// </summary>
public record SubtractValueFromSample : Command
{
    public required int Value { get; init; }
}
