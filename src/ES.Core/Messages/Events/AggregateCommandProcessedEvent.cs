using System.Diagnostics.CodeAnalysis;

namespace ES.Core.Messages.Events;

public record AggregateCommandProcessedEvent : Event
{
    public required long Version { get; init; }

    public AggregateCommandProcessedEvent() { }

    [SetsRequiredMembers]
    public AggregateCommandProcessedEvent(Command command, long version)
        : base(command)
    {
        Version = version;
    }
}
