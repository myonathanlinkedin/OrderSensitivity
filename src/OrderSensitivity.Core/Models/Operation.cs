using OrderSensitivity.Core.Models;

namespace OrderSensitivity.Core.Models;

/// <summary>
/// Metadata about an operation.
/// </summary>
public record OperationMetadata
{
    public string Description { get; init; } = string.Empty;
    public Dictionary<string, object> Parameters { get; init; } = new();
}

/// <summary>
/// Interface for operations that transform state.
/// Operations explicitly declare order sensitivity.
/// </summary>
public interface IOperation
{
    /// <summary>
    /// Name of the operation.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Whether this operation is order-sensitive.
    /// Order-sensitive operations produce different results depending on execution order.
    /// </summary>
    bool IsOrderSensitive { get; }

    /// <summary>
    /// Executes the operation on the current state, producing a new state.
    /// </summary>
    State Execute(State currentState);

    /// <summary>
    /// Metadata about the operation.
    /// </summary>
    OperationMetadata Metadata { get; }
}

/// <summary>
/// Base class for operations with common functionality.
/// </summary>
public abstract class OperationBase : IOperation
{
    public abstract string Name { get; }
    public abstract bool IsOrderSensitive { get; }
    public abstract State Execute(State currentState);
    
    public virtual OperationMetadata Metadata => new()
    {
        Description = $"Operation: {Name}",
        Parameters = new Dictionary<string, object>()
    };
}


