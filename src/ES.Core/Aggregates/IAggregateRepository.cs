using ES.Core.Models;

namespace ES.Core.Aggregates;

/// <summary>
/// Repository for loading aggregate instances
/// </summary>
/// <typeparam name="TAggregate">Aggreate type</typeparam>
/// <typeparam name="TState">Aggregate state</typeparam>
public interface IAggregateRepository<TAggregate, TState>
    where TAggregate : IAggregate<TState>, IAggregateConstructor<TAggregate, TState>
    where TState : VersionedModel
{
    /// <summary>
    /// Loads the aggregate event stream
    /// </summary>
    /// <param name="streamId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TAggregate> LoadAsync(Guid streamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads a list of aggregate event streams
    /// </summary>
    /// <param name="streamIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<TAggregate> LoadAsync(IEnumerable<Guid> streamIds, CancellationToken cancellationToken = default);
}
