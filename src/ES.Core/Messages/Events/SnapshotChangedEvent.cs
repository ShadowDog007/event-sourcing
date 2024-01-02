using ES.Core.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ES.Core.Messages.Events;

public record SnapshotChangedEvent : Event
{
    public required string Type { get; init; }
    public JsonElement Snapshot { get; init; }
    public bool Deleted { get; init; }
}

public record SnapshotChangedEvent<TSnapshot> : SnapshotChangedEvent
    where TSnapshot : VersionedModel
{
    public new required TSnapshot Snapshot { get; init; }

    [SetsRequiredMembers]
    public SnapshotChangedEvent(TSnapshot snapshot)
    {
        Type = typeof(TSnapshot).FullName!;
        Snapshot = snapshot;
    }
}
