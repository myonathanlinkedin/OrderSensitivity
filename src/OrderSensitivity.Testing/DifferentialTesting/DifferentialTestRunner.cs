using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Utilities;

namespace OrderSensitivity.Testing.DifferentialTesting;

/// <summary>
/// Runs differential tests to compare behavior under different orders.
/// </summary>
public class DifferentialTestRunner
{
    /// <summary>
    /// Tests differential behavior under different operation orders.
    /// </summary>
    public DifferentialTestResult TestDifferential(
        IReadOnlyList<IOperation> operations,
        State initialState)
    {
        var orders = DifferentialTestGenerator.GenerateDifferentOrders(operations).ToList();
        var results = new List<StateTransition>();

        foreach (var sequence in orders)
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

        // Analyze differences
        var differences = AnalyzeDifferences(results);

        return new DifferentialTestResult
        {
            Results = results,
            Differences = differences,
            HasOrderSensitivity = differences.Count > 0
        };
    }

    private List<StateDifference> AnalyzeDifferences(List<StateTransition> results)
    {
        var differences = new List<StateDifference>();

        for (int i = 0; i < results.Count; i++)
        {
            for (int j = i + 1; j < results.Count; j++)
            {
                var diff = StateComparer.GetDifference(
                    results[i].FinalState,
                    results[j].FinalState);

                if (diff.HasDifferences)
                {
                    differences.Add(diff);
                }
            }
        }

        return differences;
    }
}

/// <summary>
/// Result of differential testing.
/// </summary>
public record DifferentialTestResult
{
    public List<StateTransition> Results { get; init; } = new();
    public List<StateDifference> Differences { get; init; } = new();
    public bool HasOrderSensitivity { get; init; }
}


