using System;
using OrderSensitivity.Core.Models;

namespace OrderSensitivity.Core.Models;

/// <summary>
/// Represents a sequence of operations to be executed in order.
/// </summary>
public class OperationSequence
{
    public IReadOnlyList<IOperation> Operations { get; }
    public ExecutionOrder Order { get; }

    public OperationSequence(IEnumerable<IOperation> operations)
    {
        if (operations == null)
        {
            throw new ArgumentNullException(nameof(operations));
        }

        Operations = operations.ToList();
        Order = new ExecutionOrder(Operations);
    }

    public OperationSequence(IReadOnlyList<IOperation> operations, ExecutionOrder order)
    {
        Operations = operations;
        Order = order;
    }

    /// <summary>
    /// Executes the sequence of operations on an initial state.
    /// </summary>
    public State Execute(State initialState)
    {
        if (!IsValid())
        {
            throw new InvalidOperationException("Operation sequence is not valid");
        }

        var currentState = initialState;
        foreach (var operation in Operations)
        {
            currentState = operation.Execute(currentState);
        }
        return currentState;
    }

    /// <summary>
    /// Checks if the sequence is valid before execution.
    /// </summary>
    public bool IsValid()
    {
        if (Operations.Count == 0)
        {
            return false;
        }

        if (!Order.IsValid())
        {
            return false;
        }

        // Check for constraint violations
        for (int i = 0; i < Operations.Count; i++)
        {
            if (Order.ViolatesConstraints(Operations[i], i))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Creates a new sequence with operations in a different order.
    /// </summary>
    public OperationSequence WithDifferentOrder(IEnumerable<IOperation> reorderedOperations)
    {
        return new OperationSequence(reorderedOperations);
    }
}


