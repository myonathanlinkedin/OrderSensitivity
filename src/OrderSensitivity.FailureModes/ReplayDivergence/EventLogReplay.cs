using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Systems;
using OrderSensitivity.Core.Utilities;

namespace OrderSensitivity.FailureModes.ReplayDivergence;

/// <summary>
/// Event log replay system for demonstrating replay divergence.
/// </summary>
public class EventLogReplay
{
    private readonly EventSourcingSystem _system;

    public EventLogReplay(EventSourcingSystem system)
    {
        _system = system;
    }

    /// <summary>
    /// Replays events in original order.
    /// </summary>
    public State ReplayOriginal(Func<Event, IOperation> eventToOperation)
    {
        return _system.ReplayAll(eventToOperation);
    }

    /// <summary>
    /// Replays events in different order (by timestamp).
    /// </summary>
    public State ReplayInDifferentOrder(Func<Event, IOperation> eventToOperation)
    {
        var events = _system.EventLog;
        return _system.ReplayInDifferentOrder(events, eventToOperation, Comparer<Event>.Create((e1, e2) => e1.Timestamp.CompareTo(e2.Timestamp)));
    }

    /// <summary>
    /// Replays events in reverse order.
    /// </summary>
    public State ReplayReverse(Func<Event, IOperation> eventToOperation)
    {
        var events = _system.EventLog.Reverse();
        return _system.ReplayEvents(events, eventToOperation);
    }

    /// <summary>
    /// Checks if replay produces different state.
    /// </summary>
    public bool HasReplayDivergence(
        Func<Event, IOperation> eventToOperation,
        State originalState)
    {
        var replayState = ReplayOriginal(eventToOperation);
        return !StateComparer.AreEqual(originalState, replayState);
    }
}

