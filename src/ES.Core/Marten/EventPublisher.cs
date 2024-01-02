using ES.Core.MassTransit;
using Marten;
using Marten.Services;
using MassTransit;

namespace ES.Core.Marten;

/// <summary>
/// Publishes all events saved to the document session
/// </summary>
/// <param name="consumeContextProvider"></param>
/// <param name="bus"></param>
public class EventPublisher(IConsumeContextProvider consumeContextProvider, IBus bus) : IDocumentSessionListener
{
    public void AfterCommit(IDocumentSession session, IChangeSet commit)
    {
        AfterCommitAsync(session, commit, default).GetAwaiter().GetResult();
    }

    public async Task AfterCommitAsync(IDocumentSession session, IChangeSet commit, CancellationToken token)
    {
        // If we're in a consumer scope publish to outbox, otherwise publish directly to bus
        var endpoint = consumeContextProvider.Current is { } context
            ? (IPublishEndpoint)context
            // TODO - Look into using a transactional bus
            : bus;

        IEnumerable<object> events = commit.GetEvents()
            .Select(s => s.Data)
            .OfType<Event>();

        await endpoint.PublishBatch(events, cancellationToken: token);
    }

    public void BeforeSaveChanges(IDocumentSession session) { }
    public Task BeforeSaveChangesAsync(IDocumentSession session, CancellationToken token) => Task.CompletedTask;
    public void DocumentAddedForStorage(object id, object document) { }
    public void DocumentLoaded(object id, object document) { }
}
