using MassTransit;

namespace ES.Core.MassTransit;

public class ConsumeContextScopeFilter<T> : IFilter<ConsumeContext<T>>
    where T : class
{
    public void Probe(ProbeContext context)
    {
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        using var scope = new ConsumeContextScope(context);
        await next.Send(context);
    }
}
