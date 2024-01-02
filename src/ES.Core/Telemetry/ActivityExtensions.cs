using ES.Core.Messages;
using System.Diagnostics;

namespace ES.Core.Telemetry;

public static class ActivityExtensions
{
    public static void AddMessageTags(this Activity activity, Message message)
    {
        if (message is Event @event)
        {
            activity.AddEventTags(@event);
        }
        else if (message is Command command)
        {
            activity.AddCommandTags(command);
        }
        else
        {
            activity.AddMessageCommonTags(message);
        }
    }

    public static void AddCommandTags(this Activity activity, Command command)
    {
        activity.AddMessageCommonTags(command);

        activity.AddTag(nameof(command.StreamId), command.StreamId);
        if (command.ExpectedVersion.HasValue)
            activity.AddTag(nameof(command.ExpectedVersion), command.ExpectedVersion.Value);
    }

    public static void AddEventTags(this Activity activity, Event @event)
    {
        activity.AddMessageCommonTags(@event);

        activity.AddTag(nameof(@event.StreamId), @event.StreamId);
    }

    private static void AddMessageCommonTags(this Activity activity, Message message)
    {
        ArgumentNullException.ThrowIfNull(activity);
        ArgumentNullException.ThrowIfNull(message);

        activity.AddTag(nameof(Message.MessageId), message.MessageId);
        activity.AddTag(nameof(Message.ConversationId), message.ConversationId);
        activity.AddTag(nameof(Message.CorrelationId), message.CorrelationId);
    }
}
