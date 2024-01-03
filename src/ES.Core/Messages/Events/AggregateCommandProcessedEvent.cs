using System.Diagnostics.CodeAnalysis;

namespace ES.Core.Messages.Events;

/// <summary>
/// Published after the successful processing of a command
/// </summary>
public record AggregateCommandProcessedEvent : Event
{
    /// <summary>
    /// The version of the aggregate which has processed this event
    /// </summary>
    public required long Version { get; init; }

    /// <summary>
    /// True if the command was processed with validation only enabled
    /// </summary>
    public bool ValidationOnly { get; init; }

    public AggregateCommandProcessedEvent() { }

    [SetsRequiredMembers]
    public AggregateCommandProcessedEvent(Command command, long version)
        : base(command)
    {
        Version = version;
    }
}
