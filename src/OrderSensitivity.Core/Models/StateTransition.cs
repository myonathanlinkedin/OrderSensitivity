using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Utilities;

namespace OrderSensitivity.Core.Models;

/// <summary>
/// Represents the result of a state transition.
/// </summary>
public record StateTransition
{
    public State InitialState { get; init; } = new();
    public State FinalState { get; init; } = new();
    public IReadOnlyList<IOperation> Operations { get; init; } = new List<IOperation>();
    public ExecutionOrder Order { get; init; } = new ExecutionOrder(Array.Empty<int>());

    /// <summary>
    /// Checks if this transition produces the same final state as another transition.
    /// </summary>
    public bool ProducesSameState(StateTransition other)
    {
        return StateComparer.AreEqual(FinalState, other.FinalState);
    }

    /// <summary>
    /// Gets the differences between this transition and another.
    /// </summary>
    public StateDifference GetDifference(StateTransition other)
    {
        return StateComparer.GetDifference(FinalState, other.FinalState);
    }
}

/// <summary>
/// Represents differences between two states.
/// </summary>
public record StateDifference
{
    public List<string> DifferentProperties { get; init; } = new();
    public Dictionary<string, (object? Original, object? Other)> PropertyDifferences { get; init; } = new();
    public bool HasDifferences => DifferentProperties.Count > 0 || PropertyDifferences.Count > 0;
}

