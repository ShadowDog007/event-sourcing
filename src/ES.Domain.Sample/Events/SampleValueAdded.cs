using ES.Core.Messages;
using ES.Domain.Sample.Commands;
using System.Diagnostics.CodeAnalysis;

namespace ES.Domain.Sample.Events;

/// <summary>
/// Record of an addition to Sample
/// </summary>
public record SampleValueAdded : Event
{
    public required int Value { get; init; }

    public SampleValueAdded() {}

    [SetsRequiredMembers]
    public SampleValueAdded(AddValueToSample command)
        : base(command)
    {
        Value = command.Value;
    }
}
