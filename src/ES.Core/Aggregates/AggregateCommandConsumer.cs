using ES.Core.Messages;
using ES.Core.Messages.Events;
using ES.Core.Models;
using FluentValidation;
using Marten;
using MassTransit;

namespace ES.Core.Aggregates;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TAggregate">The aggregate being processed</typeparam>
/// <typeparam name="TState"></typeparam>
/// <typeparam name="TCommand"></typeparam>
/// <param name="repository"></param>
/// <param name="handler"></param>
/// <param name="session"></param>
public sealed class AggregateCommandConsumer<TAggregate, TState, TCommand>(
    IAggregateRepository<TAggregate, TState> repository,
    IAggregateHandler<TAggregate, TCommand> handler,
    IDocumentSession session)
    : IConsumer<TCommand>
    where TAggregate : IAggregate<TState>, IAggregateHandles<TCommand>, IAggregateConstructor<TAggregate, TState>
    where TState : VersionedModel
    where TCommand : Command
{
    public async Task Consume(ConsumeContext<TCommand> context)
    {
        // Load the aggregate
        var aggregate = await repository.LoadAsync(context.Message.StreamId, context.CancellationToken);

        // Handle the command
        try
        {
            await handler.HandleAsync(aggregate, context.Message, context.CancellationToken);
        }
        catch (ValidationException ex)
        {
            var response = new AggregateCommandFailedEvent<TCommand>(context.Message, ex.Errors);

            if (context.RequestId is not null)
            {
                await context.RespondAsync(response);
            }
            else
            {
                await context.Publish(response);
            }
            return;
        }

        // Save events
        if (context.Message.ValidationOnly)
        {
            await session.SaveChangesAsync(context.CancellationToken);
        }

        // If command is a request, respond with new version
        if (context.RequestId is not null)
        {
            await context.RespondAsync(new AggregateCommandProcessedEvent(context.Message, aggregate.Stream.CurrentVersion ?? 1));
        }
    }
}
