using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Systems;
using OrderSensitivity.Examples.UserAccount;

namespace OrderSensitivity.FailureModes.EventOrderingMistakes;

/// <summary>
/// Distributed event processor that can process events out of order.
/// </summary>
public class DistributedEventProcessor
{
    /// <summary>
    /// Processes events in the order provided.
    /// </summary>
    public State ProcessEvents(IEnumerable<Event> events, State initialState)
    {
        var state = initialState;
        var eventToOperation = new Func<Event, IOperation>(evt => evt.Type switch
        {
            "Deposit" => new DepositOperation(Convert.ToDecimal(evt.Data["Amount"])),
            "ApplyFee" => new ApplyFeeOperation(Convert.ToDecimal(evt.Data["FeePercentage"])),
            "Withdraw" => new WithdrawOperation(Convert.ToDecimal(evt.Data["Amount"])),
            _ => throw new InvalidOperationException($"Unknown event type: {evt.Type}")
        });

        foreach (var evt in events)
        {
            var operation = eventToOperation(evt);
            state = operation.Execute(state);
        }

        return state;
    }

    /// <summary>
    /// Processes events as they arrive (potentially out of order).
    /// </summary>
    public State ProcessAsArrived(IEnumerable<Event> events, State initialState)
    {
        // Process events in arrival order (which may be wrong)
        return ProcessEvents(events, initialState);
    }

    /// <summary>
    /// Processes events in correct order (by timestamp).
    /// </summary>
    public State ProcessInCorrectOrder(IEnumerable<Event> events, State initialState)
    {
        var orderedEvents = events.OrderBy(e => e.Timestamp);
        return ProcessEvents(orderedEvents, initialState);
    }
}


