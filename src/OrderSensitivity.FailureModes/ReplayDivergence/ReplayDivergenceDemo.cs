using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Systems;
using OrderSensitivity.Core.Utilities;
using OrderSensitivity.Examples.UserAccount;

namespace OrderSensitivity.FailureModes.ReplayDivergence;

/// <summary>
/// Demonstrates replay divergence - replay produces different state than original execution.
/// </summary>
public class ReplayDivergenceDemo
{
    public ReplayDivergenceResult Demonstrate(State initialState)
    {
        // Original execution
        var originalSequence = new OperationSequence(new IOperation[]
        {
            new DepositOperation(100m),
            new ApplyFeeOperation(0.1m)
        });
        var originalState = originalSequence.Execute(initialState);

        // Replay with events in different order
        var replaySequence = new OperationSequence(new IOperation[]
        {
            new ApplyFeeOperation(0.1m),  // Replayed in different order
            new DepositOperation(100m)
        });
        var replayState = replaySequence.Execute(initialState);

        // States differ - replay divergence
        var hasDivergence = !StateComparer.AreEqual(originalState, replayState);
        var difference = StateComparer.GetDifference(originalState, replayState);

        return new ReplayDivergenceResult
        {
            OriginalState = originalState,
            ReplayState = replayState,
            HasDivergence = hasDivergence,
            Difference = difference
        };
    }
}

/// <summary>
/// Result of replay divergence demonstration.
/// </summary>
public record ReplayDivergenceResult
{
    public State OriginalState { get; init; } = new();
    public State ReplayState { get; init; } = new();
    public bool HasDivergence { get; init; }
    public StateDifference Difference { get; init; } = new();
}

