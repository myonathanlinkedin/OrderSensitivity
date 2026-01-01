using OrderSensitivity.Core.Models;

namespace OrderSensitivity.Testing.SequenceTesting;

/// <summary>
/// Runs sequence tests to check order sensitivity.
/// </summary>
public class SequenceTestRunner
{
    /// <summary>
    /// Tests all sequences and compares results.
    /// </summary>
    public SequenceTestResult TestSequences(
        IReadOnlyList<IOperation> operations,
        State initialState)
    {
        var sequences = SequenceTestGenerator.GeneratePermutations(operations).ToList();
        var results = new List<StateTransition>();

        foreach (var sequence in sequences)
        {
            var finalState = sequence.Execute(initialState);
            results.Add(new StateTransition
            {
                InitialState = initialState,
                FinalState = finalState,
                Operations = sequence.Operations,
                Order = sequence.Order
            });
        }

        // Check if all final states are the same
        var distinctStates = results.Select(r => r.FinalState).Distinct().ToList();
        var hasOrderSensitivity = distinctStates.Count > 1;

        return new SequenceTestResult
        {
            Results = results,
            HasOrderSensitivity = hasOrderSensitivity,
            DistinctFinalStates = distinctStates.Count,
            TotalSequences = sequences.Count
        };
    }
}

/// <summary>
/// Result of sequence testing.
/// </summary>
public record SequenceTestResult
{
    public List<StateTransition> Results { get; init; } = new();
    public bool HasOrderSensitivity { get; init; }
    public int DistinctFinalStates { get; init; }
    public int TotalSequences { get; init; }
}


