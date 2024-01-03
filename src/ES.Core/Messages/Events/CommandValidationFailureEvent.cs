using FluentValidation.Results;
using System.Diagnostics.CodeAnalysis;

namespace ES.Core.Messages.Events;

/// <summary>
/// Published when validation of <typeparamref name="TCommand"/> fails
/// </summary>
/// <typeparam name="TCommand"></typeparam>
public record CommandValidationFailureEvent<TCommand> : Event
    where TCommand : Command
{
    /// <summary>
    /// The command which triggered the validation failure
    /// </summary>
    public required TCommand Command { get; init; }

    /// <summary>
    /// The validations failures
    /// </summary>
    public required IReadOnlyList<ValidationFailure> Failures { get; init; } = [];

    public CommandValidationFailureEvent() { }

    [SetsRequiredMembers]
    public CommandValidationFailureEvent(TCommand command, IEnumerable<ValidationFailure> failures)
        : base(command)
    {
        Command = command;
        Failures = [..failures];
    }
}
