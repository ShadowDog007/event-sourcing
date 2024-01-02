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

    protected Guid? DeterministicStreamId() => null;
}
