using OrderSensitivity.Core.Models;

namespace OrderSensitivity.Testing.SequenceTesting;

/// <summary>
/// Generates operation sequences for testing.
/// </summary>
public static class SequenceTestGenerator
{
    /// <summary>
    /// Generates all permutations of operations.
    /// </summary>
    public static IEnumerable<OperationSequence> GeneratePermutations(IReadOnlyList<IOperation> operations)
    {
        if (operations.Count == 0)
        {
            yield break;
        }

        if (operations.Count == 1)
        {
            yield return new OperationSequence(operations);
            yield break;
        }

        // Generate all permutations
        var indices = Enumerable.Range(0, operations.Count).ToArray();
        var permutations = GetPermutations(indices);

        foreach (var permutation in permutations)
        {
            var reordered = permutation.Select(i => operations[i]).ToList();
            yield return new OperationSequence(reordered);
        }
    }

    /// <summary>
    /// Generates a subset of permutations (for large operation sets).
    /// </summary>
    public static IEnumerable<OperationSequence> GenerateSamplePermutations(
        IReadOnlyList<IOperation> operations,
        int maxPermutations = 100)
    {
        var count = 0;
        foreach (var sequence in GeneratePermutations(operations))
        {
            if (count >= maxPermutations)
            {
                yield break;
            }
            yield return sequence;
            count++;
        }
    }

    private static IEnumerable<int[]> GetPermutations(int[] array)
    {
        if (array.Length == 1)
        {
            yield return array;
            yield break;
        }

        for (int i = 0; i < array.Length; i++)
        {
            var remaining = array.Where((_, index) => index != i).ToArray();
            foreach (var perm in GetPermutations(remaining))
            {
                yield return new[] { array[i] }.Concat(perm).ToArray();
            }
        }
    }
}


