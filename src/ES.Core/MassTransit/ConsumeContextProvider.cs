using MassTransit;

namespace ES.Core.MassTransit;

public class ConsumeContextProvider : IConsumeContextProvider
{
    public static AsyncLocal<ConsumeContext> ConsumeContext { get; } = new();

    public ConsumeContext? Current => ConsumeContext.Value;
}
