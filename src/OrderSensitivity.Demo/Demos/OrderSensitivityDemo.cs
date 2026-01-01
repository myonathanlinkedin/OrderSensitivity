using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Utilities;
using OrderSensitivity.Examples.UserAccount;

namespace OrderSensitivity.Demo.Demos;

/// <summary>
/// Demonstrates order sensitivity patterns.
/// </summary>
public static class OrderSensitivityDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Order Sensitivity Patterns Demo ===");
        Console.WriteLine();

        // Example 1: User Account - Order-sensitive operations
        Console.WriteLine("Example 1: User Account System");
        Console.WriteLine("----------------------------------------");
        
        var account = AccountState.Create(0m);
        var deposit = new DepositOperation(100m);
        var applyFee = new ApplyFeeOperation(0.1m);

        // Order 1: Deposit then Fee
        var sequence1 = new OperationSequence(new IOperation[] { deposit, applyFee });
        var result1 = sequence1.Execute(account);
        Console.WriteLine($"Order 1: Deposit(100) then ApplyFee(10%)");
        Console.WriteLine($"  Final Balance: {AccountState.GetBalance(result1):C}");
        Console.WriteLine();

        // Order 2: Fee then Deposit
        var sequence2 = new OperationSequence(new IOperation[] { applyFee, deposit });
        var result2 = sequence2.Execute(account);
        Console.WriteLine($"Order 2: ApplyFee(10%) then Deposit(100)");
        Console.WriteLine($"  Final Balance: {AccountState.GetBalance(result2):C}");
        Console.WriteLine();

        // Compare results
        var hasOrderSensitivity = !StateComparer.AreEqual(result1, result2);
        Console.WriteLine($"Order Sensitivity: {hasOrderSensitivity}");
        if (hasOrderSensitivity)
        {
            var difference = StateComparer.GetDifference(result1, result2);
            Console.WriteLine($"  Differences: {difference.DifferentProperties.Count} property(ies) differ");
            foreach (var prop in difference.DifferentProperties)
            {
                var (val1, val2) = difference.PropertyDifferences[prop];
                Console.WriteLine($"    {prop}: {val1} vs {val2}");
            }
        }
        Console.WriteLine();
    }
}


