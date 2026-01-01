using OrderSensitivity.Core.Models;
using OrderSensitivity.Examples.UserAccount;
using Xunit;

namespace OrderSensitivity.Examples.Tests;

public class UserAccountTests
{
    [Fact]
    public void DepositAndApplyFee_AreOrderSensitive()
    {
        var initialState = AccountState.Create(0m);
        var deposit = new DepositOperation(100m);
        var applyFee = new ApplyFeeOperation(0.1m);

        var sequence1 = new OperationSequence(new IOperation[] { deposit, applyFee });
        var sequence2 = new OperationSequence(new IOperation[] { applyFee, deposit });

        var finalState1 = sequence1.Execute(initialState);
        var finalState2 = sequence2.Execute(initialState);

        // Final states should differ
        Assert.NotEqual(finalState1, finalState2);
        
        var balance1 = AccountState.GetBalance(finalState1);
        var balance2 = AccountState.GetBalance(finalState2);
        
        Assert.Equal(90m, balance1);  // 100 - 10% = 90
        Assert.Equal(100m, balance2); // 0 - 0% + 100 = 100
    }

    [Fact]
    public void DepositOperations_AreOrderInsensitive()
    {
        var initialState = AccountState.Create(0m);
        var deposit1 = new DepositOperation(100m);
        var deposit2 = new DepositOperation(50m);

        var sequence1 = new OperationSequence(new IOperation[] { deposit1, deposit2 });
        var sequence2 = new OperationSequence(new IOperation[] { deposit2, deposit1 });

        var finalState1 = sequence1.Execute(initialState);
        var finalState2 = sequence2.Execute(initialState);

        // Final states should have the same balance (order-insensitive)
        var balance1 = AccountState.GetBalance(finalState1);
        var balance2 = AccountState.GetBalance(finalState2);
        Assert.Equal(balance1, balance2);
        Assert.Equal(150m, balance1);
    }
}


