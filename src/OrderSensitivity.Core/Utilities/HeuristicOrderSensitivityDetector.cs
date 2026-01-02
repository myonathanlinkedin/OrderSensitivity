using OrderSensitivity.Core.Models;

namespace OrderSensitivity.Core.Utilities;

/// <summary>
/// Result of heuristic-based order sensitivity detection.
/// </summary>
public record HeuristicDetectionResult
{
    public bool IsOrderSensitive { get; init; }
    public List<(IOperation, IOperation)> OrderSensitivePairs { get; init; } = new();
    public int PairsTested { get; init; }
    public TimeSpan DetectionTime { get; init; }
}

/// <summary>
/// Heuristic-based approaches for detecting order sensitivity.
/// Reduces complexity from O(n!·T) to O(n²·T) or O(d·T) where d is number of dependencies.
/// </summary>
public static class HeuristicOrderSensitivityDetector
{
    /// <summary>
    /// Dependency-based heuristic: test only critical operation pairs based on dependency graph.
    /// Complexity: O(d·T) where d is number of dependencies.
    /// </summary>
    public static HeuristicDetectionResult DetectDependencyBased(
        IReadOnlyList<IOperation> operations,
        State initialState)
    {
        if (operations == null)
        {
            throw new ArgumentNullException(nameof(operations));
        }

        if (operations.Count < 2)
        {
            return new HeuristicDetectionResult
            {
                IsOrderSensitive = false,
                PairsTested = 0
            };
        }

        var startTime = DateTime.UtcNow;
        var dependencyGraph = BuildDependencyGraph(operations);
        var criticalPairs = IdentifyCriticalPairs(dependencyGraph, operations);
        var orderSensitivePairs = new List<(IOperation, IOperation)>();

        foreach (var (op1, op2) in criticalPairs)
        {
            var state1 = op2.Execute(op1.Execute(initialState));
            var state2 = op1.Execute(op2.Execute(initialState));
            
            if (!StateComparer.AreEqual(state1, state2))
            {
                orderSensitivePairs.Add((op1, op2));
            }
        }

        var detectionTime = DateTime.UtcNow - startTime;

        return new HeuristicDetectionResult
        {
            IsOrderSensitive = orderSensitivePairs.Count > 0,
            OrderSensitivePairs = orderSensitivePairs,
            PairsTested = criticalPairs.Count,
            DetectionTime = detectionTime
        };
    }

    /// <summary>
    /// State-based heuristic: test all operation pairs.
    /// Complexity: O(n²·T) where n is number of operations.
    /// </summary>
    public static HeuristicDetectionResult DetectStateBased(
        IReadOnlyList<IOperation> operations,
        State initialState)
    {
        if (operations == null)
        {
            throw new ArgumentNullException(nameof(operations));
        }

        if (operations.Count < 2)
        {
            return new HeuristicDetectionResult
            {
                IsOrderSensitive = false,
                PairsTested = 0
            };
        }

        var startTime = DateTime.UtcNow;
        var orderSensitivePairs = new List<(IOperation, IOperation)>();
        var pairsTested = 0;

        for (int i = 0; i < operations.Count; i++)
        {
            for (int j = i + 1; j < operations.Count; j++)
            {
                pairsTested++;
                var op1 = operations[i];
                var op2 = operations[j];

                var state1 = op2.Execute(op1.Execute(initialState));
                var state2 = op1.Execute(op2.Execute(initialState));

                if (!StateComparer.AreEqual(state1, state2))
                {
                    orderSensitivePairs.Add((op1, op2));
                }
            }
        }

        var detectionTime = DateTime.UtcNow - startTime;

        return new HeuristicDetectionResult
        {
            IsOrderSensitive = orderSensitivePairs.Count > 0,
            OrderSensitivePairs = orderSensitivePairs,
            PairsTested = pairsTested,
            DetectionTime = detectionTime
        };
    }

    /// <summary>
    /// Hybrid approach: combines dependency-based and state-based heuristics.
    /// </summary>
    public static HeuristicDetectionResult DetectHybrid(
        IReadOnlyList<IOperation> operations,
        State initialState)
    {
        if (operations == null)
        {
            throw new ArgumentNullException(nameof(operations));
        }

        // First, use dependency-based heuristic for critical pairs
        var dependencyResult = DetectDependencyBased(operations, initialState);

        if (dependencyResult.IsOrderSensitive)
        {
            return dependencyResult;
        }

        // If no order sensitivity found in critical pairs, test remaining pairs
        var dependencyGraph = BuildDependencyGraph(operations);
        var criticalPairs = IdentifyCriticalPairs(dependencyGraph, operations);
        var criticalPairSet = new HashSet<(IOperation, IOperation)>(criticalPairs);

        var startTime = DateTime.UtcNow;
        var orderSensitivePairs = new List<(IOperation, IOperation)>(dependencyResult.OrderSensitivePairs);
        var pairsTested = dependencyResult.PairsTested;

        for (int i = 0; i < operations.Count; i++)
        {
            for (int j = i + 1; j < operations.Count; j++)
            {
                var pair = (operations[i], operations[j]);
                if (criticalPairSet.Contains(pair))
                {
                    continue; // Already tested
                }

                pairsTested++;
                var state1 = operations[j].Execute(operations[i].Execute(initialState));
                var state2 = operations[i].Execute(operations[j].Execute(initialState));

                if (!StateComparer.AreEqual(state1, state2))
                {
                    orderSensitivePairs.Add(pair);
                }
            }
        }

        var detectionTime = DateTime.UtcNow - startTime;

        return new HeuristicDetectionResult
        {
            IsOrderSensitive = orderSensitivePairs.Count > 0,
            OrderSensitivePairs = orderSensitivePairs,
            PairsTested = pairsTested,
            DetectionTime = detectionTime
        };
    }

    /// <summary>
    /// Builds a dependency graph from operations.
    /// Operations that read before write, or have explicit dependencies, create edges.
    /// </summary>
    private static Dictionary<IOperation, List<IOperation>> BuildDependencyGraph(
        IReadOnlyList<IOperation> operations)
    {
        var graph = new Dictionary<IOperation, List<IOperation>>();

        foreach (var op in operations)
        {
            graph[op] = new List<IOperation>();
        }

        // Identify dependencies based on operation characteristics
        for (int i = 0; i < operations.Count; i++)
        {
            var op1 = operations[i];

            // If operation is order-sensitive, it may depend on others
            if (op1.IsOrderSensitive)
            {
                for (int j = 0; j < operations.Count; j++)
                {
                    if (i != j)
                    {
                        var op2 = operations[j];
                        // Heuristic: order-sensitive operations likely depend on state-modifying operations
                        if (op2.IsOrderSensitive || op1.Name != op2.Name)
                        {
                            graph[op1].Add(op2);
                        }
                    }
                }
            }
        }

        return graph;
    }

    /// <summary>
    /// Identifies critical operation pairs for testing based on dependency graph.
    /// </summary>
    private static List<(IOperation, IOperation)> IdentifyCriticalPairs(
        Dictionary<IOperation, List<IOperation>> graph,
        IReadOnlyList<IOperation> operations)
    {
        var pairs = new List<(IOperation, IOperation)>();

        // Add pairs where dependencies exist
        foreach (var (op1, dependencies) in graph)
        {
            foreach (var op2 in dependencies)
            {
                // Add both (op1, op2) and (op2, op1) to test both orders
                if (!pairs.Contains((op1, op2)) && !pairs.Contains((op2, op1)))
                {
                    pairs.Add((op1, op2));
                }
            }
        }

        // If no dependencies found, add all pairs of order-sensitive operations
        if (pairs.Count == 0)
        {
            var orderSensitiveOps = operations.Where(op => op.IsOrderSensitive).ToList();
            for (int i = 0; i < orderSensitiveOps.Count; i++)
            {
                for (int j = i + 1; j < orderSensitiveOps.Count; j++)
                {
                    pairs.Add((orderSensitiveOps[i], orderSensitiveOps[j]));
                }
            }
        }

        return pairs;
    }
}


