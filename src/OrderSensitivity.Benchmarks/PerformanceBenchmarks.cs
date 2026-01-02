using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Utilities;
using OrderSensitivity.Examples.UserAccount;
using System.Diagnostics;

namespace OrderSensitivity.Benchmarks;

/// <summary>
/// Benchmarks for performance evaluation of order sensitivity detection and validation.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class PerformanceBenchmarks
{
    private State _initialState = null!;
    private List<IOperation> _operations10 = null!;
    private List<IOperation> _operations100 = null!;
    private List<IOperation> _operations1000 = null!;
    private List<IOperation> _operations10000 = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Initialize state with sufficient balance for withdrawal operations
        _initialState = new State().WithProperty("Balance", 100000m);
        _operations10 = GenerateOperations(10);
        _operations100 = GenerateOperations(100);
        _operations1000 = GenerateOperations(1000);
        _operations10000 = GenerateOperations(10000);
    }

    private List<IOperation> GenerateOperations(int count)
    {
        var operations = new List<IOperation>();
        for (int i = 0; i < count; i++)
        {
            // Mix of order-sensitive and order-insensitive operations
            if (i % 3 == 0)
            {
                operations.Add(new DepositOperation(10m));
            }
            else if (i % 3 == 1)
            {
                operations.Add(new ApplyFeeOperation(0.05m));
            }
            else
            {
                operations.Add(new WithdrawOperation(5m));
            }
        }
        return operations;
    }

    [Benchmark]
    [Arguments(10)]
    [Arguments(100)]
    [Arguments(1000)]
    [Arguments(10000)]
    public void ExecuteSequence(int sequenceLength)
    {
        var operations = sequenceLength switch
        {
            10 => _operations10,
            100 => _operations100,
            1000 => _operations1000,
            10000 => _operations10000,
            _ => throw new ArgumentException("Invalid sequence length")
        };

        var sequence = new OperationSequence(operations);
        var _ = sequence.Execute(_initialState);
    }

    [Benchmark]
    [Arguments(10)]
    [Arguments(100)]
    [Arguments(1000)]
    [Arguments(10000)]
    public void ExecuteSequenceWithValidation(int sequenceLength)
    {
        var operations = sequenceLength switch
        {
            10 => _operations10,
            100 => _operations100,
            1000 => _operations1000,
            10000 => _operations10000,
            _ => throw new ArgumentException("Invalid sequence length")
        };

        var sequence = new OperationSequence(operations);
        var constraints = new List<OrderingConstraint>();
        OrderValidator.CheckSequence(sequence, constraints);
        var _ = sequence.Execute(_initialState);
    }

    [Benchmark]
    public void HeuristicDetectionDependencyBased()
    {
        HeuristicOrderSensitivityDetector.DetectDependencyBased(_operations100, _initialState);
    }

    [Benchmark]
    public void HeuristicDetectionStateBased()
    {
        HeuristicOrderSensitivityDetector.DetectStateBased(_operations100, _initialState);
    }

    [Benchmark]
    public void OptimizedDetectionWithCaching()
    {
        var detector = new OptimizedOrderSensitivityDetector(
            enableEarlyTermination: true,
            enableParallelTesting: false);
        detector.DetectWithCaching(_operations100, _initialState);
    }

    [Benchmark]
    public void OptimizedDetectionParallel()
    {
        var detector = new OptimizedOrderSensitivityDetector(
            enableEarlyTermination: true,
            enableParallelTesting: true);
        detector.DetectWithCaching(_operations100, _initialState);
    }
}

/// <summary>
/// Manual timing benchmarks for detailed performance analysis.
/// </summary>
public class ManualPerformanceBenchmarks
{
    public static (double ExecutionTime, double MemoryMB) MeasureExecution(
        IReadOnlyList<IOperation> operations,
        State initialState)
    {
        var stopwatch = Stopwatch.StartNew();
        var memoryBefore = GC.GetTotalMemory(false);

        var sequence = new OperationSequence(operations);
        var result = sequence.Execute(initialState);

        var memoryAfter = GC.GetTotalMemory(false);
        stopwatch.Stop();

        var executionTime = stopwatch.Elapsed.TotalMilliseconds;
        var memoryMB = (memoryAfter - memoryBefore) / (1024.0 * 1024.0);

        return (executionTime, memoryMB);
    }

    public static (double ExecutionTime, double OverheadPercent) MeasureOverhead(
        IReadOnlyList<IOperation> operations,
        State initialState)
    {
        // Without validation
        var stopwatch = Stopwatch.StartNew();
        var sequence = new OperationSequence(operations);
        var _ = sequence.Execute(initialState);
        stopwatch.Stop();
        var timeWithoutValidation = stopwatch.Elapsed.TotalMilliseconds;

        // With validation
        stopwatch.Restart();
        var constraints = new List<OrderingConstraint>();
        OrderValidator.CheckSequence(sequence, constraints);
        var __ = sequence.Execute(initialState);
        stopwatch.Stop();
        var timeWithValidation = stopwatch.Elapsed.TotalMilliseconds;

        var overhead = ((timeWithValidation - timeWithoutValidation) / timeWithoutValidation) * 100.0;

        return (timeWithValidation, overhead);
    }
}

/// <summary>
/// Program entry point for running benchmarks.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Order Sensitivity Performance Benchmarks");
        Console.WriteLine("========================================");
        Console.WriteLine();

        if (args.Length > 0 && args[0] == "manual")
        {
            // Run manual benchmarks (faster, for paper)
            ManualBenchmarkRunner.RunAllBenchmarks();
        }
        else
        {
            // Run BenchmarkDotNet benchmarks (more detailed, slower)
            Console.WriteLine("Running BenchmarkDotNet benchmarks (this may take a while)...");
            var summary = BenchmarkRunner.Run<PerformanceBenchmarks>();
            Console.WriteLine();
            Console.WriteLine("Benchmarks completed. See results above.");
        }
    }
}


