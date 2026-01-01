using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Utilities;

namespace OrderSensitivity.Testing.ReplayTesting;

/// <summary>
/// Runs replay tests to verify replay correctness.
/// </summary>
public class ReplayTestRunner
{
    /// <summary>
    /// Tests replay of a sequence.
    /// </summary>
    public ReplayTestResult TestReplay(
        OperationSequence originalSequence,
        State initialState)
    {
        // Original execution
        var originalState = originalSequence.Execute(initialState);

        // Record sequence
        var recorder = new ReplayTestRecorder();
        recorder.RecordSequence(originalSequence, initialState, originalState);

        // Replay sequence
        var recordedSequence = recorder.GetRecordedSequence();
        var replayState = recordedSequence.Execute(initialState);

        // Check for divergence
        var hasDivergence = !StateComparer.AreEqual(originalState, replayState);
        var difference = StateComparer.GetDifference(originalState, replayState);

        return new ReplayTestResult
        {
            OriginalState = originalState,
            ReplayState = replayState,
            HasDivergence = hasDivergence,
            Difference = difference
        };
    }
}

/// <summary>
/// Result of replay testing.
/// </summary>
public record ReplayTestResult
{
    public State OriginalState { get; init; } = new();
    public State ReplayState { get; init; } = new();
    public bool HasDivergence { get; init; }
    public StateDifference Difference { get; init; } = new();
}


