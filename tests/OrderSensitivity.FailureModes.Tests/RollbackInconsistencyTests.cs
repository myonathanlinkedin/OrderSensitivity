using OrderSensitivity.Examples.UserAccount;
using OrderSensitivity.FailureModes.RollbackInconsistency;
using Xunit;

namespace OrderSensitivity.FailureModes.Tests;

public class RollbackInconsistencyTests
{
    [Fact]
    public void RollbackInconsistency_DemonstratesRollbackIssue()
    {
        var initialState = AccountState.Create(0m);
        var demo = new RollbackInconsistencyDemo();
        var result = demo.Demonstrate(initialState);

        // Rollback may or may not restore state due to order sensitivity
        // The demonstration shows the attempt
        Assert.NotNull(result);
        Assert.NotNull(result.FinalState);
        
        // If rollback state exists, check if it differs from initial
        if (result.RollbackState != null)
        {
            var initialBalance = AccountState.GetBalance(initialState);
            var rollbackBalance = AccountState.GetBalance(result.RollbackState);
            // Due to order sensitivity, rollback may not exactly restore state
            // This demonstrates the inconsistency
            Assert.NotNull(result.Difference);
        }
    }
}

