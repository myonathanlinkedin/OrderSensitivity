using OrderSensitivity.Examples.UserAccount;
using OrderSensitivity.FailureModes.ReplayDivergence;

namespace OrderSensitivity.Demo.Demos;

/// <summary>
/// Demonstrates failure modes.
/// </summary>
public static class FailureModesDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Failure Modes Demonstration ===");
        Console.WriteLine();

        // Replay Divergence
        Console.WriteLine("Failure Mode 1: Replay Divergence");
        Console.WriteLine("----------------------------------------");
        
        var initialState = AccountState.Create(0m);
        var demo = new ReplayDivergenceDemo();
        var result = demo.Demonstrate(initialState);

        Console.WriteLine($"Original State Balance: {AccountState.GetBalance(result.OriginalState):C}");
        Console.WriteLine($"Replay State Balance: {AccountState.GetBalance(result.ReplayState):C}");
        Console.WriteLine($"Has Divergence: {result.HasDivergence}");
        
        if (result.HasDivergence)
        {
            Console.WriteLine($"  Differences: {result.Difference.DifferentProperties.Count} property(ies) differ");
        }
        Console.WriteLine();
    }
}


