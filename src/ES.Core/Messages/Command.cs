namespace ES.Core.Messages;

public record Command : Message
{
    private readonly Guid _streamId;

    /// <summary>
    /// The stream this command impacts
    /// </summary>
    public Guid StreamId
    {
        get => DeterministicStreamId() ?? _streamId;
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

    protected Guid? DeterministicStreamId() => null;
}
