using ES.Core.Aggregates;
using ES.Core.Aggregates.Validation;
using ES.Core.Messages;
using ES.Core.Messages.Events;
using ES.Core.Models;
using ES.Core.Telemetry;
using FluentValidation;
using FluentValidation.Results;
using Marten;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ES.Core.Api;

public static class EndpointHelpers
{
    /// <summary>
    /// Gets the aggregates current state
    /// </summary>
    /// <typeparam name="TAggregate"></typeparam>
    /// <typeparam name="TState"></typeparam>
    /// <param name="streamId"></param>
    /// <param name="repository"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<Results<Ok<TState>, NotFound>> GetAggregateStateAsync<TAggregate, TState>(Guid streamId,
        IAggregateRepository<TAggregate, TState> repository,
        CancellationToken cancellationToken = default)
        where TAggregate : IAggregate<TState>, IAggregateConstructor<TAggregate, TState>
        where TState : VersionedModel
    {
        var aggregate = await repository.LoadAsync(streamId, cancellationToken);

        if (!aggregate.Exists)
            return TypedResults.NotFound();

        return TypedResults.Ok(aggregate.State);
    }

    public static async Task<Results<
            Ok<AggregateCommandProcessedEvent>, NotFound,
            BadRequest<AggregateCommandFailedEvent<TCommand>>
        >>
        AppendToStreamAsync<TAggregate, TState, TCommand>(
        [FromRoute] Guid streamId, [FromBody] TCommand command,
        IAggregateRepository<TAggregate, TState> repository,
        IAggregateHandler<TAggregate, TCommand> commandHandler,
        IDocumentSession session,
        CancellationToken cancellationToken = default)
        where TAggregate : IAggregate<TState>, IAggregateConstructor<TAggregate, TState>, IAggregateHandles<TCommand>
        where TState : VersionedModel
        where TCommand : Command
    {
        Activity.Current?.AddMessageTags(command);

        if (command.StreamId != default && command.StreamId != streamId)
        {
            return TypedResults.BadRequest(new AggregateCommandFailedEvent<TCommand>(command, [
                new ValidationFailure("StreamId", "StreamId property does not match streamId from route")
                {
                    AttemptedValue = streamId,
                    Severity = Severity.Error,
                }
            ]));
        }

        var aggregate = await repository.LoadAsync(command.StreamId, cancellationToken);

        if (command is not ICommandSupportsStart && !aggregate.Exists)
            return TypedResults.NotFound();

        try
        {
            await commandHandler.HandleAsync(aggregate, command, cancellationToken);
        }
        catch (ValidationException ex)
        {
            return TypedResults.BadRequest(new AggregateCommandFailedEvent<TCommand>(command, ex.Errors));
        }

        if (command.MessageId.ToString()[0] == '5')
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        if (!command.ValidationOnly)
        {
            await session.SaveChangesAsync(cancellationToken);
        }

        return TypedResults.Ok(new AggregateCommandProcessedEvent(command, aggregate.Stream.CurrentVersion ?? 1));
    }

    public static Task<Results<
            Ok<AggregateCommandProcessedEvent>, NotFound,
            BadRequest<AggregateCommandFailedEvent<TCommand>>
        >>
        CreateStreamAsync<TAggregate, TState, TCommand>(
        [FromBody] TCommand command,
        IAggregateRepository<TAggregate, TState> repository,
        IAggregateHandler<TAggregate, TCommand> commandHandler,
        IDocumentSession session,
        CancellationToken cancellationToken = default)
        where TAggregate : IAggregate<TState>, IAggregateConstructor<TAggregate, TState>, IAggregateHandles<TCommand>
        where TState : VersionedModel
        where TCommand : Command, ICommandStartsStream
    {
        return AppendToStreamAsync(command.StreamId, command, repository, commandHandler, session, cancellationToken);
    }

}
