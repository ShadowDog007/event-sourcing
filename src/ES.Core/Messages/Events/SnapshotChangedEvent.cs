using ES.Core.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ES.Core.Messages.Events;

/// <summary>
/// Published when an aggregate projection is updated
/// </summary>
public record SnapshotChangedEvent : Event
{
    /// <summary>
    /// The FQN of the projection state type
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// JSON of the new projection state
    /// </summary>
    public JsonElement Snapshot { get; init; }

    /// <summary>
    /// Flag if the snapshot was deleted
    /// </summary>
    public bool Deleted { get; init; }
}

/// <summary>
/// Published when an aggregate projection of type <typeparamref name="TSnapshot"/> is updated
/// </summary>
/// <typeparam name="TSnapshot"></typeparam>
public record SnapshotChangedEvent<TSnapshot> : SnapshotChangedEvent
    where TSnapshot : VersionedModel
{
    /// <summary>
    /// The new projection state
    /// </summary>
    public new required TSnapshot Snapshot { get; init; }

    [SetsRequiredMembers]
    public SnapshotChangedEvent(TSnapshot snapshot)
    {
        Type = typeof(TSnapshot).FullName!;
        Snapshot = snapshot;
    }
}
