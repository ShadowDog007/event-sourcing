using MassTransit;
using System.Transactions;

namespace ES.Core.MassTransit;

public class TransactionScopeFilter<T> : IFilter<ConsumeContext<T>>
    where T : class
{
    public void Probe(ProbeContext context)
    {
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        using var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);
        await next.Send(context);
    }
}
