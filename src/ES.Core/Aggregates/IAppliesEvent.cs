using ES.Core.Messages;
using ES.Core.Models;

namespace ES.Core.Projections;

public interface IAppliesEvent<TState, TEvent>
    where TState : VersionedModel
    where TEvent : Event
{
    /// <summary>
    /// Applies the event to the current state and produces the next state
    /// </summary>
    /// <param name="current">The current state</param>
    /// <param name="event">The event being applied</param>
    /// <returns>The next state after applying the provided event</returns>
    TState Apply(TState current, TEvent @event);
}
