using System.Diagnostics.CodeAnalysis;

namespace ES.Core.Messages;

/// <summary>
/// Record of an event occurance
/// </summary>
public record Event : Message
{
    /// <summary>
    /// The stream this event applies to
    /// </summary>
    public required Guid StreamId { get; init; }

    /// <summary>
    /// The time this event was processed
    /// </summary>
    public required DateTimeOffset EventTime { get; init; }

    public Event() { }

    [SetsRequiredMembers]
    protected Event(Command command)
        : base(command)
    {
        StreamId = command.StreamId;
        EventTime = DateTimeOffset.Now;
    }
}
