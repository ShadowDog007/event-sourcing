using MassTransit;

namespace ES.Core.MassTransit;

public class ConsumeContextScope : IDisposable
{
    public ConsumeContextScope(ConsumeContext context)
    {
        ConsumeContextProvider.ConsumeContext.Value = context;
    }

    public void Dispose()
    {
        ConsumeContextProvider.ConsumeContext.Value = null!;
    }
}
