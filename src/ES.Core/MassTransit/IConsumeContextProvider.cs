using MassTransit;

namespace ES.Core.MassTransit;

public interface IConsumeContextProvider
{
    ConsumeContext? Current { get; }
}
