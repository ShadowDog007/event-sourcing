using ES.Core.Models;
using Marten;
using System.Runtime.CompilerServices;

namespace ES.Core.Aggregates;

public class AggregateRepository<TAggregate, TState>(IDocumentSession session) : IAggregateRepository<TAggregate, TState>
    where TAggregate : IAggregate<TState>, IAggregateConstructor<TAggregate, TState>
    where TState : VersionedModel
{
    public async Task<TAggregate> LoadAsync(Guid streamId, CancellationToken cancellationToken = default)
    {
        var stream = await session.Events.FetchForWriting<TState>(streamId, cancellationToken);
        var state = stream.Aggregate;
        
        // If this aggregate isn't up to date
        if (stream.Aggregate == null && stream.CurrentVersion > 0
            || stream.Aggregate != null && stream.CurrentVersion > stream.Aggregate.Version)
        {
            // then catch up
            state = await session.Events.AggregateStreamAsync<TState>(streamId, token: cancellationToken);
        }

        return TAggregate.Create(stream, state);
    }

    public async IAsyncEnumerable<TAggregate> LoadAsync(IEnumerable<Guid> streamIds, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var streamId in streamIds)
        {
            yield return await LoadAsync(streamId, cancellationToken);
        }
    }
}
