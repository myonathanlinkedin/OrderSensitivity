using OrderSensitivity.Core.Models;

namespace OrderSensitivity.Core.Patterns;

/// <summary>
/// Base class for order-sensitive operations.
/// Order-sensitive operations produce different results depending on execution order.
/// </summary>
public abstract class OrderSensitiveOperation : OperationBase
{
    public override bool IsOrderSensitive => true;
}

/// <summary>
/// Base class for order-insensitive operations.
/// Order-insensitive operations produce the same result regardless of execution order.
/// </summary>
public abstract class OrderInsensitiveOperation : OperationBase
{
    public override bool IsOrderSensitive => false;
}

/// <summary>
/// Base class for state-dependent operations.
/// State-dependent operations read current state and their effect depends on what state they see.
/// </summary>
public abstract class StateDependentOperation : OrderSensitiveOperation
{
    /// <summary>
    /// Gets the value from state that this operation depends on.
    /// </summary>
    protected abstract object? GetDependentValue(State currentState);

    /// <summary>
    /// Checks if the dependent value exists in the state.
    /// </summary>
    protected bool HasDependentValue(State currentState)
    {
        return GetDependentValue(currentState) != null;
    }
}


