using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Systems;
using OrderSensitivity.Core.Utilities;
using OrderSensitivity.Examples.UserAccount;

namespace OrderSensitivity.FailureModes.EventOrderingMistakes;

/// <summary>
/// Demonstrates event ordering mistakes - events processed out of order produce incorrect state.
/// </summary>
public class EventOrderingMistakesDemo
{
    public EventOrderingMistakesResult Demonstrate(State initialState)
    {
        // Events arrive out of order
        var events = new[]
        {
            new Event
            {
                Type = "Deposit",
                Data = new Dictionary<string, object> { ["Amount"] = 100m },
                Timestamp = DateTime.UtcNow.AddSeconds(2)
            },
            new Event
            {
                Type = "ApplyFee",
                Data = new Dictionary<string, object> { ["FeePercentage"] = 0.1m },
                Timestamp = DateTime.UtcNow.AddSeconds(1)
            }
        };

        // Process events in correct order (by timestamp)
        var processor = new DistributedEventProcessor();
        var state1 = processor.ProcessEvents(events.OrderBy(e => e.Timestamp), initialState);

        // Process events in wrong order (by arrival/reverse timestamp)
        var state2 = processor.ProcessEvents(events.OrderByDescending(e => e.Timestamp), initialState);

        // States differ - ordering mistake
        var hasMistake = !StateComparer.AreEqual(state1, state2);
        var difference = StateComparer.GetDifference(state1, state2);

        return new EventOrderingMistakesResult
        {
            Events = events,
            CorrectOrderState = state1,
            WrongOrderState = state2,
            HasMistake = hasMistake,
            Difference = difference
        };
    }
}

/// <summary>
/// Result of event ordering mistakes demonstration.
/// </summary>
public record EventOrderingMistakesResult
{
    public Event[] Events { get; init; } = Array.Empty<Event>();
    public State CorrectOrderState { get; init; } = new();
    public State WrongOrderState { get; init; } = new();
    public bool HasMistake { get; init; }
    public StateDifference Difference { get; init; } = new();
}


