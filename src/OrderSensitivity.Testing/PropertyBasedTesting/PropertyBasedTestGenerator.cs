using OrderSensitivity.Core.Models;

namespace OrderSensitivity.Testing.PropertyBasedTesting;

/// <summary>
/// Generates random operation sequences for property-based testing.
/// </summary>
public static class PropertyBasedTestGenerator
{
    /// <summary>
    /// Generates a random sequence of operations.
    /// </summary>
    public static OperationSequence GenerateRandomSequence(
        IReadOnlyList<IOperation> availableOperations,
        Random random,
        int? length = null)
    {
        var sequenceLength = length ?? random.Next(1, availableOperations.Count + 1);
        var selected = new List<IOperation>();

        for (int i = 0; i < sequenceLength; i++)
        {
            var index = random.Next(availableOperations.Count);
            selected.Add(availableOperations[index]);
        }

        return new OperationSequence(selected);
    }

    /// <summary>
    /// Generates multiple random sequences.
    /// </summary>
    public static IEnumerable<OperationSequence> GenerateRandomSequences(
        IReadOnlyList<IOperation> availableOperations,
        Random random,
        int count,
        int? minLength = null,
        int? maxLength = null)
    {
        for (int i = 0; i < count; i++)
        {
            var length = random.Next(
                minLength ?? 1,
                (maxLength ?? availableOperations.Count) + 1);
            yield return GenerateRandomSequence(availableOperations, random, length);
        }
    }
}


