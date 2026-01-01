using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Utilities;
using Xunit;

namespace OrderSensitivity.Core.Tests.Models;

public class StateTransitionTests
{
    [Fact]
    public void ProducesSameState_WithSameFinalStates_ReturnsTrue()
    {
        var finalState = new State().WithProperty("Key", "Value");
        var transition1 = new StateTransition { FinalState = finalState };
        var transition2 = new StateTransition { FinalState = finalState };

        Assert.True(transition1.ProducesSameState(transition2));
    }

    [Fact]
    public void ProducesSameState_WithDifferentFinalStates_ReturnsFalse()
    {
        var state1 = new State().WithProperty("Key", "Value1");
        var state2 = new State().WithProperty("Key", "Value2");
        var transition1 = new StateTransition { FinalState = state1 };
        var transition2 = new StateTransition { FinalState = state2 };

        Assert.False(transition1.ProducesSameState(transition2));
    }

    [Fact]
    public void GetDifference_WithDifferentStates_ReturnsDifferences()
    {
        var state1 = new State().WithProperty("Key", "Value1");
        var state2 = new State().WithProperty("Key", "Value2");
        var transition1 = new StateTransition { FinalState = state1 };
        var transition2 = new StateTransition { FinalState = state2 };

        var difference = transition1.GetDifference(transition2);

        Assert.True(difference.HasDifferences);
        Assert.Contains("Key", difference.DifferentProperties);
    }

    [Fact]
    public void GetDifference_WithSameStates_ReturnsNoDifferences()
    {
        var state = new State().WithProperty("Key", "Value");
        var transition1 = new StateTransition { FinalState = state };
        var transition2 = new StateTransition { FinalState = state };

        var difference = transition1.GetDifference(transition2);

        Assert.False(difference.HasDifferences);
    }
}

