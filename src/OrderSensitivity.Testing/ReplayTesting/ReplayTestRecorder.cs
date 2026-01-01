using OrderSensitivity.Core.Models;

namespace OrderSensitivity.Testing.ReplayTesting;

/// <summary>
/// Records operation sequences for replay testing.
/// </summary>
public class ReplayTestRecorder
{
    private readonly List<RecordedOperation> _recordedOperations = new();

    /// <summary>
    /// Records an operation execution.
    /// </summary>
    public void Record(IOperation operation, State stateBefore, State stateAfter)
    {
        _recordedOperations.Add(new RecordedOperation
        {
            Operation = operation,
            StateBefore = stateBefore,
            StateAfter = stateAfter,
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Records a sequence of operations.
    /// </summary>
    public void RecordSequence(OperationSequence sequence, State initialState, State finalState)
    {
        var currentState = initialState;
        foreach (var operation in sequence.Operations)
        {
            var nextState = operation.Execute(currentState);
            Record(operation, currentState, nextState);
            currentState = nextState;
        }
    }

    /// <summary>
    /// Gets the recorded sequence.
    /// </summary>
    public OperationSequence GetRecordedSequence()
    {
        var operations = _recordedOperations.Select(r => r.Operation).ToList();
        return new OperationSequence(operations);
    }

    /// <summary>
    /// Gets all recorded operations.
    /// </summary>
    public IReadOnlyList<RecordedOperation> GetRecordedOperations()
    {
        return _recordedOperations.AsReadOnly();
    }

    /// <summary>
    /// Clears recorded operations.
    /// </summary>
    public void Clear()
    {
        _recordedOperations.Clear();
    }
}

/// <summary>
/// Represents a recorded operation.
/// </summary>
public record RecordedOperation
{
    public IOperation Operation { get; init; } = null!;
    public State StateBefore { get; init; } = new();
    public State StateAfter { get; init; } = new();
    public DateTime Timestamp { get; init; }
}


