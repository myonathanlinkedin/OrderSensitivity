using OrderSensitivity.Core.Models;
using System.Collections.Concurrent;

namespace OrderSensitivity.Core.Utilities;

/// <summary>
/// Result of optimized order sensitivity detection with caching.
/// </summary>
public record OptimizedDetectionResult
{
    public bool IsOrderSensitive { get; init; }
    public List<(IOperation, IOperation)> OrderSensitivePairs { get; init; } = new();
    public int PairsTested { get; init; }
    public int CacheHits { get; init; }
    public TimeSpan DetectionTime { get; init; }
}

/// <summary>
/// Optimized order sensitivity detection with caching, early termination, and parallel testing.
/// </summary>
public class OptimizedOrderSensitivityDetector
{
    private readonly ConcurrentDictionary<(IOperation, State), State> _stateCache = new();
    private readonly bool _enableEarlyTermination;
    private readonly bool _enableParallelTesting;

    public OptimizedOrderSensitivityDetector(
        bool enableEarlyTermination = true,
        bool enableParallelTesting = false)
    {
        _enableEarlyTermination = enableEarlyTermination;
        _enableParallelTesting = enableParallelTesting;
    }

    /// <summary>
    /// Detects order sensitivity with caching optimization.
    /// </summary>
    public OptimizedDetectionResult DetectWithCaching(
        IReadOnlyList<IOperation> operations,
        State initialState)
    {
        if (operations == null)
        {
            throw new ArgumentNullException(nameof(operations));
        }

        if (operations.Count < 2)
        {
            return new OptimizedDetectionResult
            {
                IsOrderSensitive = false,
                PairsTested = 0,
                CacheHits = 0
            };
        }

        var startTime = DateTime.UtcNow;
        var orderSensitivePairs = new ConcurrentBag<(IOperation, IOperation)>();
        var pairsTested = 0;
        var cacheHits = 0;

        if (_enableParallelTesting)
        {
            // Parallel testing for independent operation pairs
            Parallel.For(0, operations.Count, i =>
            {
                for (int j = i + 1; j < operations.Count; j++)
                {
                    Interlocked.Increment(ref pairsTested);
                    var op1 = operations[i];
                    var op2 = operations[j];

                    var state1 = GetCachedOrCompute(op1, GetCachedOrCompute(op2, initialState, ref cacheHits), ref cacheHits);
                    var state2 = GetCachedOrCompute(op2, GetCachedOrCompute(op1, initialState, ref cacheHits), ref cacheHits);

                    if (!StateComparer.AreEqual(state1, state2))
                    {
                        orderSensitivePairs.Add((op1, op2));
                        
                        if (_enableEarlyTermination)
                        {
                            // Early termination: found order sensitivity, can stop
                            return;
                        }
                    }
                }
            });
        }
        else
        {
            // Sequential testing with caching
            for (int i = 0; i < operations.Count; i++)
            {
                for (int j = i + 1; j < operations.Count; j++)
                {
                    pairsTested++;
                    var op1 = operations[i];
                    var op2 = operations[j];

                    var state1 = GetCachedOrCompute(op1, GetCachedOrCompute(op2, initialState, ref cacheHits), ref cacheHits);
                    var state2 = GetCachedOrCompute(op2, GetCachedOrCompute(op1, initialState, ref cacheHits), ref cacheHits);

                    if (!StateComparer.AreEqual(state1, state2))
                    {
                        orderSensitivePairs.Add((op1, op2));
                        
                        if (_enableEarlyTermination)
                        {
                            // Early termination: found order sensitivity, can stop
                            break;
                        }
                    }
                }

                if (_enableEarlyTermination && orderSensitivePairs.Count > 0)
                {
                    break;
                }
            }
        }

        var detectionTime = DateTime.UtcNow - startTime;

        return new OptimizedDetectionResult
        {
            IsOrderSensitive = orderSensitivePairs.Count > 0,
            OrderSensitivePairs = orderSensitivePairs.ToList(),
            PairsTested = pairsTested,
            CacheHits = cacheHits,
            DetectionTime = detectionTime
        };
    }

    /// <summary>
    /// Gets cached state transition or computes and caches it.
    /// </summary>
    private State GetCachedOrCompute(IOperation operation, State state, ref int cacheHits)
    {
        var key = (operation, state);

        // Note: Using state equality for caching. In practice, might need custom equality comparer
        // For now, using reference equality for state (immutable records should work)
        if (_stateCache.TryGetValue(key, out var cachedState))
        {
            Interlocked.Increment(ref cacheHits);
            return cachedState;
        }

        var result = operation.Execute(state);
        _stateCache.TryAdd(key, result);
        return result;
    }

    /// <summary>
    /// Clears the state cache.
    /// </summary>
    public void ClearCache()
    {
        _stateCache.Clear();
    }

    /// <summary>
    /// Gets cache statistics.
    /// </summary>
    public (int CacheSize, int MaxCacheSize) GetCacheStats()
    {
        return (_stateCache.Count, _stateCache.Count);
    }
}


