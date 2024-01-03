using ES.Core.Messages;
using ES.Domain.Sample.Commands;
using System.Diagnostics.CodeAnalysis;

namespace ES.Domain.Sample.Events;

/// <summary>
/// Record of an subtraction to Sample
/// </summary>
public record SampleValueSubtracted : Event
{
    public required int Value { get; init; }

    public SampleValueSubtracted() { }

    [SetsRequiredMembers]
    public SampleValueSubtracted(SubtractValueFromSample command)
        : base(command)
    {
        Value = command.Value;
    }
}
