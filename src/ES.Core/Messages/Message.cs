namespace ES.Core.Messages;

public record Message
{
    public Guid MessageId { get; init; }
    public Guid CorrelationId { get; init; }
    public Guid ConversationId { get; init; }
}
