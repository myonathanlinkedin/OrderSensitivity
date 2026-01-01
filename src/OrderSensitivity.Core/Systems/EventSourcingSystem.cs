using System;
using OrderSensitivity.Core.Models;

namespace OrderSensitivity.Core.Systems;

/// <summary>
/// Represents an event in an event sourcing system.
/// </summary>
public record Event
{
    public string Type { get; init; } = string.Empty;
    public Dictionary<string, object> Data { get; init; } = new();
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public int SequenceNumber { get; init; }
}

/// <summary>
/// Event sourcing system that maintains an event log and reconstructs state by replaying events.
/// </summary>
public class EventSourcingSystem
{
    private readonly List<Event> _eventLog = new();
    private State _currentState;

    public IReadOnlyList<Event> EventLog => _eventLog.AsReadOnly();
    public State CurrentState => _currentState;

    public EventSourcingSystem(State initialState)
    {
        _currentState = initialState;
    }

    public EventSourcingSystem() : this(new State())
    {
    }

    /// <summary>
    /// Appends an event to the event log and applies it to the current state.
    /// </summary>
    public void AppendEvent(Event evt, IOperation operation)
    {
        if (evt == null)
        {
            throw new ArgumentNullException(nameof(evt));
        }

        if (operation == null)
        {
            throw new ArgumentNullException(nameof(operation));
        }

        var eventWithSequence = evt with { SequenceNumber = _eventLog.Count };
        _eventLog.Add(eventWithSequence);
        _currentState = operation.Execute(_currentState);
    }

    /// <summary>
    /// Replays events from the event log to reconstruct state.
    /// </summary>
    public State ReplayEvents(IEnumerable<Event> events, Func<Event, IOperation> eventToOperation)
    {
        if (events == null)
        {
            throw new ArgumentNullException(nameof(events));
        }

        if (eventToOperation == null)
        {
            throw new ArgumentNullException(nameof(eventToOperation));
        }

        var state = new State();
        foreach (var evt in events.OrderBy(e => e.SequenceNumber))
        {
            var operation = eventToOperation(evt);
            state = operation.Execute(state);
        }
        return state;
    }

    /// <summary>
    /// Replays all events from the event log.
    /// </summary>
    public State ReplayAll(Func<Event, IOperation> eventToOperation)
    {
        return ReplayEvents(_eventLog, eventToOperation);
    }

    /// <summary>
    /// Replays events in a different order (for testing replay divergence).
    /// </summary>
    public State ReplayInDifferentOrder(
        IEnumerable<Event> events,
        Func<Event, IOperation> eventToOperation,
        IComparer<Event>? comparer = null)
    {
        var orderedEvents = comparer != null
            ? events.OrderBy(e => e, comparer).ToList()
            : events.OrderBy(e => e.Timestamp).ToList();

        return ReplayEvents(orderedEvents, eventToOperation);
    }
}


