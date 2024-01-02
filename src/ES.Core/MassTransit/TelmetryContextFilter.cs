using ES.Core.Messages;
using ES.Core.Telemetry;
using MassTransit;
using System.Diagnostics;

namespace ES.Core.MassTransit;

/// <summary>
/// Adds message tags to the current activity
/// </summary>
/// <typeparam name="T"></typeparam>
public class TelmetryContextFilter<T> : IFilter<ConsumeContext<T>>
    where T : Message
{
    public void Probe(ProbeContext context)
    {
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        Activity.Current?.AddMessageTags(context.Message);

        await next.Send(context);
    }
}
