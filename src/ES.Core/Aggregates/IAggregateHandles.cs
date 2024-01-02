using ES.Core.Messages;

namespace ES.Core.Aggregates;

/// <summary>
/// Marker interface to indicate which commands this aggregate handles
/// </summary>
/// <typeparam name="TCommand"></typeparam>
public interface IAggregateHandles<TCommand>
    where TCommand : Command
{
    //ValueTask HandleAsync(IServiceProvider serviceProvider, TCommand command, CancellationToken cancellationToken = default);
}
