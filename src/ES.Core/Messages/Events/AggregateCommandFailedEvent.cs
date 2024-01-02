using FluentValidation;
using FluentValidation.Results;
using System.Diagnostics.CodeAnalysis;

namespace ES.Core.Messages.Events;

public record AggregateCommandFailedEvent<TCommand> : Event
    where TCommand : Command
{
    public required TCommand Command { get; init; }
    public required IReadOnlyList<ValidationFailure> Failures { get; init; } = [];

    public AggregateCommandFailedEvent() { }

    [SetsRequiredMembers]
    public AggregateCommandFailedEvent(TCommand command, IEnumerable<ValidationFailure> failures)
        : base(command)
    {
        Command = command;
        Failures = [..failures];
    }
}
