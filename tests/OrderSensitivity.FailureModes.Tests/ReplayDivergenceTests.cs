using OrderSensitivity.Examples.UserAccount;
using OrderSensitivity.FailureModes.ReplayDivergence;
using Xunit;

namespace OrderSensitivity.FailureModes.Tests;

public class ReplayDivergenceTests
{
    [Fact]
    public void ReplayDivergence_IsObservable()
    {
        var initialState = AccountState.Create(0m);
        var demo = new ReplayDivergenceDemo();
        var result = demo.Demonstrate(initialState);

        Assert.True(result.HasDivergence);
        Assert.NotEqual(result.OriginalState, result.ReplayState);
        
        var originalBalance = AccountState.GetBalance(result.OriginalState);
        var replayBalance = AccountState.GetBalance(result.ReplayState);
        
        Assert.NotEqual(originalBalance, replayBalance);
    }
}


