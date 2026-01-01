using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Utilities;

namespace OrderSensitivity.FailureModes.RollbackInconsistency;

/// <summary>
/// Transaction rollback system that demonstrates rollback inconsistency.
/// </summary>
public class TransactionRollback
{
    private readonly List<IOperation> _operations = new();
    private State _currentState;

    public TransactionRollback(State initialState)
    {
        _currentState = initialState;
    }

    /// <summary>
    /// Executes an operation and records it.
    /// </summary>
    public State Execute(IOperation operation)
    {
        _operations.Add(operation);
        _currentState = operation.Execute(_currentState);
        return _currentState;
    }

    /// <summary>
    /// Attempts to rollback all operations.
    /// </summary>
    public State Rollback(State initialState)
    {
        // Simple rollback: reverse operations in reverse order
        // This may not work correctly for order-sensitive operations
        var rollbackState = _currentState;
        for (int i = _operations.Count - 1; i >= 0; i--)
        {
            // Attempt to reverse operation
            // Note: This is a simplified approach and may not work for all operations
            rollbackState = AttemptReverse(_operations[i], rollbackState);
        }

        return rollbackState;
    }

    /// <summary>
    /// Attempts to reverse an operation (may not be exact for order-sensitive operations).
    /// </summary>
    private State AttemptReverse(IOperation operation, State currentState)
    {
        // This is a placeholder - actual implementation would need
        // operation-specific reverse logic
        // For order-sensitive operations, simple reversal may not restore state
        return currentState;
    }

    /// <summary>
    /// Checks if rollback restores original state.
    /// </summary>
    public bool RestoresState(State initialState)
    {
        var rollbackState = Rollback(initialState);
        return StateComparer.AreEqual(initialState, rollbackState);
    }
}


