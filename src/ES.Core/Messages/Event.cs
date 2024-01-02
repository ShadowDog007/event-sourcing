using System.Diagnostics.CodeAnalysis;

namespace ES.Core.Messages;

public record Event : Message
{
    public required Guid StreamId { get; init; }
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
