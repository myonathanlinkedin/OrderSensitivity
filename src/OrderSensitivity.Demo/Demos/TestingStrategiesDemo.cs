using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Utilities;
using OrderSensitivity.Examples.UserAccount;
using OrderSensitivity.Testing.SequenceTesting;

namespace OrderSensitivity.Demo.Demos;

/// <summary>
/// Demonstrates testing strategies.
/// </summary>
public static class TestingStrategiesDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Testing Strategies Demonstration ===");
        Console.WriteLine();

        // Sequence Testing
        Console.WriteLine("Strategy 1: Sequence Testing");
        Console.WriteLine("----------------------------------------");
        
        var operations = new IOperation[]
        {
            new DepositOperation(100m),
            new ApplyFeeOperation(0.1m)
        };
        
        var initialState = AccountState.Create(0m);
        var runner = new SequenceTestRunner();
        var result = runner.TestSequences(operations, initialState);

        Console.WriteLine($"Total Sequences Tested: {result.TotalSequences}");
        Console.WriteLine($"Distinct Final States: {result.DistinctFinalStates}");
        Console.WriteLine($"Has Order Sensitivity: {result.HasOrderSensitivity}");
        Console.WriteLine();
    }
}


