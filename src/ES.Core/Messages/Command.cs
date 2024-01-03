namespace ES.Core.Messages;

/// <summary>
/// Request to modify the state of an aggregate
/// </summary>
public record Command : Message
{
    private readonly Guid _streamId;
    private readonly Guid? _deterministicStreamId;

    /// <summary>
    /// The stream this command impacts
    /// </summary>
    public Guid StreamId
    {
        get => _deterministicStreamId ?? _streamId;
        init => _streamId = value;
    }

    /// <summary>
    /// If true, only run validation, don't save any results
    /// </summary>
    public bool ValidationOnly { get; init; }

    /// <summary>
    /// If set, this command should only apply to streams with a matching version
    /// </summary>
    public long? ExpectedVersion { get; init; }

    /// <summary>
    /// Creates an 
    /// </summary>
    /// <returns>Deterministic stream id based off the properties of this command</returns>
    protected virtual Guid? DeterministicStreamId() => null;

    public Command()
    {
        // Generate the stream ID
        _deterministicStreamId = DeterministicStreamId();
    }
}
