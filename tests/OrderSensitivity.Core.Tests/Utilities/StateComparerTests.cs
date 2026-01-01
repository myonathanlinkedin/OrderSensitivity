using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Utilities;
using Xunit;

namespace OrderSensitivity.Core.Tests.Utilities;

public class StateComparerTests
{
    [Fact]
    public void AreEqual_WithSameProperties_ReturnsTrue()
    {
        var state1 = new State().WithProperty("Key1", "Value1").WithProperty("Key2", 42);
        var state2 = new State().WithProperty("Key1", "Value1").WithProperty("Key2", 42);

        Assert.True(StateComparer.AreEqual(state1, state2));
    }

    [Fact]
    public void AreEqual_WithDifferentProperties_ReturnsFalse()
    {
        var state1 = new State().WithProperty("Key1", "Value1");
        var state2 = new State().WithProperty("Key1", "Value2");

        Assert.False(StateComparer.AreEqual(state1, state2));
    }

    [Fact]
    public void AreEqual_WithMissingProperties_ReturnsFalse()
    {
        var state1 = new State().WithProperty("Key1", "Value1").WithProperty("Key2", "Value2");
        var state2 = new State().WithProperty("Key1", "Value1");

        Assert.False(StateComparer.AreEqual(state1, state2));
    }

    [Fact]
    public void AreEqual_WithSameReference_ReturnsTrue()
    {
        var state1 = new State().WithProperty("Key", "Value");
        var state2 = state1;

        Assert.True(StateComparer.AreEqual(state1, state2));
    }

    [Fact]
    public void AreEqual_WithEmptyStates_ReturnsTrue()
    {
        var state1 = new State();
        var state2 = new State();

        Assert.True(StateComparer.AreEqual(state1, state2));
    }

    [Fact]
    public void GetDifference_WithDifferentProperties_ReturnsDifferences()
    {
        var state1 = new State().WithProperty("Key1", "Value1").WithProperty("Key2", 10);
        var state2 = new State().WithProperty("Key1", "Value2").WithProperty("Key2", 10);

        var difference = StateComparer.GetDifference(state1, state2);

        Assert.True(difference.HasDifferences);
        Assert.Contains("Key1", difference.DifferentProperties);
        Assert.Equal("Value1", difference.PropertyDifferences["Key1"].Original);
        Assert.Equal("Value2", difference.PropertyDifferences["Key1"].Other);
    }

    [Fact]
    public void GetDifference_WithSameProperties_ReturnsNoDifferences()
    {
        var state1 = new State().WithProperty("Key", "Value");
        var state2 = new State().WithProperty("Key", "Value");

        var difference = StateComparer.GetDifference(state1, state2);

        Assert.False(difference.HasDifferences);
        Assert.Empty(difference.DifferentProperties);
    }

    [Fact]
    public void ProduceSameFinalState_WithSameFinalStates_ReturnsTrue()
    {
        var finalState = new State().WithProperty("Key", "Value");
        var transition1 = new StateTransition { FinalState = finalState };
        var transition2 = new StateTransition { FinalState = finalState };

        Assert.True(StateComparer.ProduceSameFinalState(transition1, transition2));
    }

    [Fact]
    public void AreEqual_WithNullStates_HandlesNull()
    {
        var state = new State().WithProperty("Key", "Value");

        Assert.False(StateComparer.AreEqual(null, state));
        Assert.False(StateComparer.AreEqual(state, null));
        Assert.True(StateComparer.AreEqual(null, null));
    }

    [Fact]
    public void GetDifference_WithNullStates_HandlesNull()
    {
        var state = new State().WithProperty("Key", "Value");

        var diff1 = StateComparer.GetDifference(null, state);
        Assert.True(diff1.HasDifferences);

        var diff2 = StateComparer.GetDifference(state, null);
        Assert.True(diff2.HasDifferences);

        var diff3 = StateComparer.GetDifference(null, null);
        Assert.False(diff3.HasDifferences);
    }
}

