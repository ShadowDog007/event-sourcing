using MassTransit;

namespace ES.Core.Messages;

/// <summary>
/// Base class for published messages
/// </summary>
public record Message : CorrelatedBy<Guid>
{
    /// <summary>
    /// Unique Id
    /// </summary>
    public Guid MessageId { get; init; }

    /// <summary>
    /// Id used to correlate related messages
    /// </summary>
    public Guid CorrelationId { get; init; }
    public Guid ConversationId { get; init; }
}
