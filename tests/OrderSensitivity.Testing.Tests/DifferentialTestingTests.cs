using OrderSensitivity.Core.Models;
using OrderSensitivity.Examples.UserAccount;
using OrderSensitivity.Testing.DifferentialTesting;
using Xunit;

namespace OrderSensitivity.Testing.Tests;

public class DifferentialTestingTests
{
    [Fact]
    public void DifferentialTestRunner_DetectsOrderSensitivity()
    {
        var operations = new IOperation[]
        {
            new DepositOperation(100m),
            new ApplyFeeOperation(0.1m)
        };
        var initialState = AccountState.Create(0m);
        var runner = new DifferentialTestRunner();

        var result = runner.TestDifferential(operations, initialState);

        Assert.True(result.HasOrderSensitivity);
        Assert.NotEmpty(result.Differences);
    }

    [Fact]
    public void DifferentialTestGenerator_GeneratesDifferentOrders()
    {
        var operations = new IOperation[]
        {
            new DepositOperation(100m),
            new ApplyFeeOperation(0.1m)
        };

        var orders = DifferentialTestGenerator.GenerateDifferentOrders(operations).ToList();

        Assert.True(orders.Count >= 2); // Should generate at least original and reverse
    }
}

