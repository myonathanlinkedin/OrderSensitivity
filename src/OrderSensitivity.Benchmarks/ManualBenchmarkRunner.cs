using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Utilities;
using OrderSensitivity.Examples.UserAccount;
using System.Diagnostics;
using System.Text.Json;

namespace OrderSensitivity.Benchmarks;

/// <summary>
/// Manual benchmark runner that collects actual performance data.
/// </summary>
public class ManualBenchmarkRunner
{
    public static void RunAllBenchmarks()
    {
        Console.WriteLine("Running Manual Performance Benchmarks...");
        Console.WriteLine("==========================================");
        Console.WriteLine();

        var results = new BenchmarkResults();

        // Scalability benchmarks
        Console.WriteLine("1. Scalability Analysis...");
        results.Scalability = RunScalabilityBenchmarks();
        PrintScalabilityResults(results.Scalability);

        // Overhead benchmarks
        Console.WriteLine("\n2. Order Validation Overhead...");
        results.Overhead = RunOverheadBenchmarks();
        PrintOverheadResults(results.Overhead);

        // Memory benchmarks
        Console.WriteLine("\n3. Memory Usage...");
        results.Memory = RunMemoryBenchmarks();
        PrintMemoryResults(results.Memory);

        // Testing strategy benchmarks
        Console.WriteLine("\n4. Testing Strategy Performance...");
        results.TestingStrategies = RunTestingStrategyBenchmarks();
        PrintTestingStrategyResults(results.TestingStrategies);

        // Save results to JSON
        var json = JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
        
        // Save to current directory (benchmark_results.json)
        var resultsPath = Path.Combine(Directory.GetCurrentDirectory(), "benchmark_results.json");
        
        File.WriteAllText(resultsPath, json);
        Console.WriteLine($"\n✅ Results saved to: {resultsPath}");

        Console.WriteLine("\n==========================================");
        Console.WriteLine("All benchmarks completed!");
    }

    private static ScalabilityResults RunScalabilityBenchmarks()
    {
        var results = new ScalabilityResults();
        // Set initial balance to ensure operations can execute
        var initialState = new State().WithProperty("Balance", 10000m);

        foreach (var length in new[] { 10, 100, 1000, 10000 })
        {
            var operations = GenerateOperations(length);
            var sequence = new OperationSequence(operations);

            // Warm-up
            sequence.Execute(initialState);

            // Measure execution time
            var sw = Stopwatch.StartNew();
            var result = sequence.Execute(initialState);
            sw.Stop();

            var executionTime = sw.Elapsed.TotalMilliseconds;
            var timePerOp = executionTime / length * 1000; // microseconds

            // Measure memory
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            var memBefore = GC.GetTotalMemory(false);
            sequence.Execute(initialState);
            var memAfter = GC.GetTotalMemory(false);
            var memoryMB = (memAfter - memBefore) / (1024.0 * 1024.0);

            results.Data.Add(new ScalabilityDataPoint
            {
                SequenceLength = length,
                ExecutionTimeMs = Math.Round(executionTime, 2),
                TimePerOperationUs = Math.Round(timePerOp, 2),
                MemoryMB = Math.Round(memoryMB, 2)
            });
        }

        return results;
    }

    private static OverheadResults RunOverheadBenchmarks()
    {
        var results = new OverheadResults();
        // Set initial balance to ensure operations can execute
        var initialState = new State().WithProperty("Balance", 10000m);

        foreach (var length in new[] { 10, 100, 1000, 10000 })
        {
            var operations = GenerateOperations(length);
            var sequence = new OperationSequence(operations);

            // Without validation
            var sw1 = Stopwatch.StartNew();
            sequence.Execute(initialState);
            sw1.Stop();
            var timeWithout = sw1.Elapsed.TotalMilliseconds;

            // With validation
            var sw2 = Stopwatch.StartNew();
            var constraints = new List<OrderingConstraint>();
            OrderValidator.CheckSequence(sequence, constraints);
            sequence.Execute(initialState);
            sw2.Stop();
            var timeWith = sw2.Elapsed.TotalMilliseconds;

            var overhead = ((timeWith - timeWithout) / timeWithout) * 100.0;

            results.Data.Add(new OverheadDataPoint
            {
                SequenceLength = length,
                WithoutValidationMs = Math.Round(timeWithout, 2),
                WithValidationMs = Math.Round(timeWith, 2),
                OverheadPercent = Math.Round(overhead, 1)
            });
        }

        return results;
    }

    private static MemoryResults RunMemoryBenchmarks()
    {
        var results = new MemoryResults();
        // Set initial balance to ensure operations can execute
        var initialState = new State().WithProperty("Balance", 10000m);

        var configs = new[]
        {
            (Length: 100, Properties: 10),
            (Length: 1000, Properties: 10),
            (Length: 1000, Properties: 100),
            (Length: 10000, Properties: 10)
        };

        foreach (var (length, propCount) in configs)
        {
            var operations = GenerateOperations(length);
            var state = initialState;
            for (int i = 0; i < propCount; i++)
            {
                state = state.WithProperty($"prop{i}", i);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            var memBefore = GC.GetTotalMemory(false);

            var sequence = new OperationSequence(operations);
            sequence.Execute(state);

            var memAfter = GC.GetTotalMemory(false);
            var memoryMB = (memAfter - memBefore) / (1024.0 * 1024.0);
            var memoryPerOp = (memoryMB * 1024) / length; // KB per operation

            results.Data.Add(new MemoryDataPoint
            {
                SequenceLength = length,
                PropertyCount = propCount,
                MemoryMB = Math.Round(memoryMB, 2),
                MemoryPerOperationKB = Math.Round(memoryPerOp, 2)
            });
        }

        return results;
    }

    private static TestingStrategyResults RunTestingStrategyBenchmarks()
    {
        var results = new TestingStrategyResults();
        // Set initial balance to ensure operations can execute
        var initialState = new State().WithProperty("Balance", 1000m);
        var operations = GenerateOperations(5); // Small set for testing strategies

        // Sequence Testing - simulate with manual permutation testing
        var sw1 = Stopwatch.StartNew();
        var seq1 = new OperationSequence(operations);
        var _ = seq1.Execute(initialState);
        var reversed = operations.ToList();
        reversed.Reverse();
        var seq2 = new OperationSequence(reversed);
        var __ = seq2.Execute(initialState);
        sw1.Stop();
        results.SequenceTesting = Math.Round(sw1.Elapsed.TotalMilliseconds, 1);

        // Property-Based Testing - simulate with random sequences
        var sw2 = Stopwatch.StartNew();
        var random = new Random();
        for (int i = 0; i < 10; i++)
        {
            var shuffled = operations.OrderBy(x => random.Next()).ToList();
            var seq = new OperationSequence(shuffled);
            _ = seq.Execute(initialState);
        }
        sw2.Stop();
        results.PropertyBasedTesting = Math.Round(sw2.Elapsed.TotalMilliseconds, 1);

        // Replay Testing - simulate replay
        var sw3 = Stopwatch.StartNew();
        var seq3 = new OperationSequence(operations);
        var state1 = seq3.Execute(initialState);
        var state2 = seq3.Execute(initialState); // Replay
        sw3.Stop();
        results.ReplayTesting = Math.Round(sw3.Elapsed.TotalMilliseconds, 1);

        // Differential Testing - compare different orders
        var sw4 = Stopwatch.StartNew();
        var seq4 = new OperationSequence(operations);
        var stateA = seq4.Execute(initialState);
        var seq5 = new OperationSequence(reversed);
        var stateB = seq5.Execute(initialState);
        StateComparer.AreEqual(stateA, stateB);
        sw4.Stop();
        results.DifferentialTesting = Math.Round(sw4.Elapsed.TotalMilliseconds, 1);

        return results;
    }

    private static List<IOperation> GenerateOperations(int count)
    {
        var operations = new List<IOperation>();
        for (int i = 0; i < count; i++)
        {
            if (i % 3 == 0)
                operations.Add(new DepositOperation(10m));
            else if (i % 3 == 1)
                operations.Add(new ApplyFeeOperation(0.05m));
            else
                operations.Add(new WithdrawOperation(5m));
        }
        return operations;
    }

    private static void PrintScalabilityResults(ScalabilityResults results)
    {
        Console.WriteLine("Sequence Length | Execution Time (ms) | Time per Op (μs) | Memory (MB)");
        Console.WriteLine("---------------|---------------------|------------------|-------------");
        foreach (var data in results.Data)
        {
            Console.WriteLine($"{data.SequenceLength,14} | {data.ExecutionTimeMs,19:F2} | {data.TimePerOperationUs,16:F2} | {data.MemoryMB,11:F2}");
        }
    }

    private static void PrintOverheadResults(OverheadResults results)
    {
        Console.WriteLine("Sequence Length | Without Validation (ms) | With Validation (ms) | Overhead (%)");
        Console.WriteLine("---------------|------------------------|---------------------|--------------");
        foreach (var data in results.Data)
        {
            Console.WriteLine($"{data.SequenceLength,14} | {data.WithoutValidationMs,23:F2} | {data.WithValidationMs,19:F2} | {data.OverheadPercent,12:F1}");
        }
    }

    private static void PrintMemoryResults(MemoryResults results)
    {
        Console.WriteLine("Sequence Length | Properties | Memory (MB) | Memory per Op (KB)");
        Console.WriteLine("---------------|------------|------------|-------------------");
        foreach (var data in results.Data)
        {
            Console.WriteLine($"{data.SequenceLength,14} | {data.PropertyCount,10} | {data.MemoryMB,10:F2} | {data.MemoryPerOperationKB,17:F2}");
        }
    }

    private static void PrintTestingStrategyResults(TestingStrategyResults results)
    {
        Console.WriteLine("Strategy | Execution Time (ms)");
        Console.WriteLine("---------|-------------------");
        Console.WriteLine($"Sequence Testing | {results.SequenceTesting:F1}");
        Console.WriteLine($"Property-Based Testing | {results.PropertyBasedTesting:F1}");
        Console.WriteLine($"Replay Testing | {results.ReplayTesting:F1}");
        Console.WriteLine($"Differential Testing | {results.DifferentialTesting:F1}");
    }
}

// Results classes
public class BenchmarkResults
{
    public ScalabilityResults Scalability { get; set; } = new();
    public OverheadResults Overhead { get; set; } = new();
    public MemoryResults Memory { get; set; } = new();
    public TestingStrategyResults TestingStrategies { get; set; } = new();
}

public class ScalabilityResults
{
    public List<ScalabilityDataPoint> Data { get; set; } = new();
}

public class ScalabilityDataPoint
{
    public int SequenceLength { get; set; }
    public double ExecutionTimeMs { get; set; }
    public double TimePerOperationUs { get; set; }
    public double MemoryMB { get; set; }
}

public class OverheadResults
{
    public List<OverheadDataPoint> Data { get; set; } = new();
}

public class OverheadDataPoint
{
    public int SequenceLength { get; set; }
    public double WithoutValidationMs { get; set; }
    public double WithValidationMs { get; set; }
    public double OverheadPercent { get; set; }
}

public class MemoryResults
{
    public List<MemoryDataPoint> Data { get; set; } = new();
}

public class MemoryDataPoint
{
    public int SequenceLength { get; set; }
    public int PropertyCount { get; set; }
    public double MemoryMB { get; set; }
    public double MemoryPerOperationKB { get; set; }
}

public class TestingStrategyResults
{
    public double SequenceTesting { get; set; }
    public double PropertyBasedTesting { get; set; }
    public double ReplayTesting { get; set; }
    public double DifferentialTesting { get; set; }
}


