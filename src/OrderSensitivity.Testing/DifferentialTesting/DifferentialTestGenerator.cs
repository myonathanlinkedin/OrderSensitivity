using OrderSensitivity.Core.Models;

namespace OrderSensitivity.Testing.DifferentialTesting;

/// <summary>
/// Generates different operation orders for differential testing.
/// </summary>
public static class DifferentialTestGenerator
{
    /// <summary>
    /// Generates different orders of the same operations.
    /// </summary>
    public static IEnumerable<OperationSequence> GenerateDifferentOrders(
        IReadOnlyList<IOperation> operations)
    {
        if (operations.Count < 2)
        {
            yield return new OperationSequence(operations);
            yield break;
        }

        // Generate a few key different orders
        yield return new OperationSequence(operations);

        // Reverse order
        yield return new OperationSequence(operations.Reverse());

        // Middle-first order (if 3+ operations)
        if (operations.Count >= 3)
        {
            var middleFirst = new List<IOperation>
            {
                operations[operations.Count / 2]
            };
            middleFirst.AddRange(operations.Where((_, i) => i != operations.Count / 2));
            yield return new OperationSequence(middleFirst);
        }

        // Last-first order (if 2+ operations)
        if (operations.Count >= 2)
        {
            var lastFirst = new List<IOperation> { operations[^1] };
            lastFirst.AddRange(operations.Take(operations.Count - 1));
            yield return new OperationSequence(lastFirst);
        }
    }
}


