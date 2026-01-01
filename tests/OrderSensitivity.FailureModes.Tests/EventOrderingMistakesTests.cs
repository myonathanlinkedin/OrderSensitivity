using OrderSensitivity.Examples.UserAccount;
using OrderSensitivity.FailureModes.EventOrderingMistakes;
using Xunit;

namespace OrderSensitivity.FailureModes.Tests;

public class EventOrderingMistakesTests
{
    [Fact]
    public void EventOrderingMistakes_WithWrongOrder_ProducesIncorrectState()
    {
        var initialState = AccountState.Create(0m);
        var demo = new EventOrderingMistakesDemo();
        var result = demo.Demonstrate(initialState);

        Assert.True(result.HasMistake);
        Assert.NotEqual(result.CorrectOrderState, result.WrongOrderState);
    }

    [Fact]
    public void EventOrderingMistakes_ShowsDifference()
    {
        var initialState = AccountState.Create(0m);
        var demo = new EventOrderingMistakesDemo();
        var result = demo.Demonstrate(initialState);

        Assert.True(result.Difference.HasDifferences);
    }
}

