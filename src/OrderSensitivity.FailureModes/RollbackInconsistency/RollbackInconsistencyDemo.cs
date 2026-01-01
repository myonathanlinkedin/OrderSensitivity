using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Patterns;
using OrderSensitivity.Core.Utilities;
using OrderSensitivity.Examples.UserAccount;

namespace OrderSensitivity.FailureModes.RollbackInconsistency;

/// <summary>
/// Demonstrates rollback inconsistency - rollback does not restore original state.
/// </summary>
public class RollbackInconsistencyDemo
{
    public RollbackInconsistencyResult Demonstrate(State initialState)
    {
        // Forward execution
        var forwardSequence = new OperationSequence(new IOperation[]
        {
            new DepositOperation(100m),
            new ApplyFeeOperation(0.1m)
        });
        var finalState = forwardSequence.Execute(initialState);

        // Attempt rollback by reversing operations
        // Note: Simply reversing order-sensitive operations may not restore state
        var rollbackSequence = new OperationSequence(new IOperation[]
        {
            new ReverseApplyFeeOperation(0.1m),  // Reverse operations
            new ReverseDepositOperation(100m)
        });

        State? rollbackState = null;
        Exception? rollbackException = null;
        try
        {
            rollbackState = rollbackSequence.Execute(finalState);
        }
        catch (Exception ex)
        {
            rollbackException = ex;
        }

        // Check if rollback restored original state
        var restored = rollbackState != null && 
                      StateComparer.AreEqual(initialState, rollbackState);
        var hasInconsistency = !restored;

        StateDifference? difference = null;
        if (rollbackState != null)
        {
            difference = StateComparer.GetDifference(initialState, rollbackState);
        }

        return new RollbackInconsistencyResult
        {
            InitialState = initialState,
            FinalState = finalState,
            RollbackState = rollbackState,
            RollbackException = rollbackException,
            Restored = restored,
            HasInconsistency = hasInconsistency,
            Difference = difference
        };
    }
}

/// <summary>
/// Reverse operations for rollback demonstration.
/// </summary>
public class ReverseDepositOperation : OrderSensitiveOperation
{
    private readonly decimal _amount;

    public ReverseDepositOperation(decimal amount)
    {
        _amount = amount;
    }

    public override string Name => $"ReverseDeposit({_amount})";

    public override State Execute(State currentState)
    {
        var balance = currentState.GetProperty<decimal>("Balance", 0m);
        return currentState.WithProperty("Balance", balance - _amount);
    }
}

public class ReverseApplyFeeOperation : OrderSensitiveOperation
{
    private readonly decimal _feePercentage;

    public ReverseApplyFeeOperation(decimal feePercentage)
    {
        _feePercentage = feePercentage;
    }

    public override string Name => $"ReverseApplyFee({_feePercentage:P})";

    public override State Execute(State currentState)
    {
        // Attempting to reverse fee calculation - this is approximate
        // and may not exactly restore state due to order sensitivity
        var balance = currentState.GetProperty<decimal>("Balance", 0m);
        var estimatedOriginalBalance = balance / (1 - _feePercentage);
        var fee = estimatedOriginalBalance * _feePercentage;
        return currentState.WithProperty("Balance", balance + fee);
    }
}

/// <summary>
/// Result of rollback inconsistency demonstration.
/// </summary>
public record RollbackInconsistencyResult
{
    public State InitialState { get; init; } = new();
    public State FinalState { get; init; } = new();
    public State? RollbackState { get; init; }
    public Exception? RollbackException { get; init; }
    public bool Restored { get; init; }
    public bool HasInconsistency { get; init; }
    public StateDifference? Difference { get; init; }
}


