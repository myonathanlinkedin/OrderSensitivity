using OrderSensitivity.Core.Models;

namespace OrderSensitivity.Core.Utilities;

/// <summary>
/// Represents a constraint on operation ordering.
/// </summary>
public record OrderingConstraint
{
    public string OperationName { get; init; } = string.Empty;
    public int? MinPosition { get; init; }
    public int? MaxPosition { get; init; }
    public List<string> MustPrecede { get; init; } = new();
    public List<string> MustFollow { get; init; } = new();
}

/// <summary>
/// Result of sequence validation.
/// </summary>
public record ValidationResult
{
    public bool IsValid { get; init; }
    public List<string> Errors { get; init; } = new();
    public List<string> Warnings { get; init; } = new();
}

/// <summary>
/// Utilities for validating operation ordering.
/// </summary>
public static class OrderValidator
{
    /// <summary>
    /// Validates a sequence against ordering constraints.
    /// </summary>
    public static ValidationResult CheckSequence(
        OperationSequence sequence,
        IEnumerable<OrderingConstraint>? constraints)
    {
        if (sequence == null)
        {
            throw new ArgumentNullException(nameof(sequence));
        }

        var errors = new List<string>();
        var warnings = new List<string>();

        var constraintMap = constraints?.ToDictionary(c => c.OperationName) ?? new Dictionary<string, OrderingConstraint>();

        for (int i = 0; i < sequence.Operations.Count; i++)
        {
            var operation = sequence.Operations[i];
            
            if (constraintMap.TryGetValue(operation.Name, out var constraint))
            {
                // Check position constraints
                if (constraint.MinPosition.HasValue && i < constraint.MinPosition.Value)
                {
                    errors.Add($"Operation {operation.Name} must be at position >= {constraint.MinPosition.Value}, but is at {i}");
                }

                if (constraint.MaxPosition.HasValue && i > constraint.MaxPosition.Value)
                {
                    errors.Add($"Operation {operation.Name} must be at position <= {constraint.MaxPosition.Value}, but is at {i}");
                }

                // Check precedence constraints
                // If operation A must precede operation B, then A must come before B
                foreach (var mustPrecede in constraint.MustPrecede)
                {
                    int precedeIndex = -1;
                    for (int j = 0; j < sequence.Operations.Count; j++)
                    {
                        if (sequence.Operations[j].Name == mustPrecede)
                        {
                            precedeIndex = j;
                            break;
                        }
                    }
                    
                    if (precedeIndex == -1)
                    {
                        warnings.Add($"Operation {mustPrecede} not found in sequence");
                    }
                    else if (i >= precedeIndex)
                    {
                        // Current operation (at i) must precede mustPrecede (at precedeIndex)
                        // But current operation comes after or at same position - violation
                        errors.Add($"Operation {operation.Name} must precede {mustPrecede}, but {operation.Name} is at position {i} and {mustPrecede} is at {precedeIndex}");
                    }
                }

                // Check follow constraints
                // If operation A must follow operation B, then B must come before A
                foreach (var mustFollow in constraint.MustFollow)
                {
                    int followIndex = -1;
                    for (int j = 0; j < sequence.Operations.Count; j++)
                    {
                        if (sequence.Operations[j].Name == mustFollow)
                        {
                            followIndex = j;
                            break;
                        }
                    }
                    
                    if (followIndex == -1)
                    {
                        warnings.Add($"Operation {mustFollow} not found in sequence");
                    }
                    else if (followIndex >= i)
                    {
                        // Current operation (at i) must follow mustFollow (at followIndex)
                        // But mustFollow comes after or at same position - violation
                        errors.Add($"Operation {operation.Name} must follow {mustFollow}, but {operation.Name} is at position {i} and {mustFollow} is at {followIndex}");
                    }
                }
            }
        }

        return new ValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors,
            Warnings = warnings
        };
    }

    /// <summary>
    /// Determines if operations are order-sensitive by testing permutations.
    /// </summary>
    public static bool IsOrderSensitive(
        IReadOnlyList<IOperation> operations,
        State initialState)
    {
        if (operations == null)
        {
            throw new ArgumentNullException(nameof(operations));
        }

        if (operations.Count < 2)
        {
            return false;
        }

        // If any operation declares itself order-sensitive, the set is order-sensitive
        if (operations.Any(op => op.IsOrderSensitive))
        {
            return true;
        }

        // Test with a simple permutation to check for observable differences
        // This is a heuristic - full verification would require testing all permutations
        var sequence1 = new OperationSequence(operations);
        var state1 = sequence1.Execute(initialState);

        var reversed = operations.Reverse().ToList();
        var sequence2 = new OperationSequence(reversed);
        var state2 = sequence2.Execute(initialState);

        return !StateComparer.AreEqual(state1, state2);
    }
}


