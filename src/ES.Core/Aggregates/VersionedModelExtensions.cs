using ES.Core.Messages;
using ES.Core.Models;

namespace ES.Core.Projections;

public static class VersionedModelExtensions
{
    public static TState Apply<TState>(this TState current, Event @event)
        where TState : VersionedModel
    {
        return current with
        {
            StreamId = @event.StreamId,
            CreatedTime = current.CreatedTime == default ? @event.EventTime : current.CreatedTime,
            ModifiedTime = @event.EventTime,
            Version = current.Version + 1,
        };
    }
}
