using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Utilities;
using Xunit;

namespace OrderSensitivity.Core.Tests.Models;

public class StateTests
{
    [Fact]
    public void WithProperty_CreatesNewState()
    {
        var state1 = new State();
        var state2 = state1.WithProperty("Key", "Value");

        Assert.NotSame(state1, state2);
        Assert.False(state1.HasProperty("Key"));
        Assert.True(state2.HasProperty("Key"));
        Assert.Equal("Value", state2.GetProperty<string>("Key"));
    }

    [Fact]
    public void GetProperty_ReturnsDefaultWhenNotFound()
    {
        var state = new State();
        var value = state.GetProperty<string>("NonExistent", "Default");

        Assert.Equal("Default", value);
    }

    [Fact]
    public void WithProperties_UpdatesMultipleProperties()
    {
        var state = new State();
        var updates = new Dictionary<string, object>
        {
            ["Key1"] = "Value1",
            ["Key2"] = "Value2"
        };

        var newState = state.WithProperties(updates);

        Assert.Equal("Value1", newState.GetProperty<string>("Key1"));
        Assert.Equal("Value2", newState.GetProperty<string>("Key2"));
    }

    [Fact]
    public void WithProperty_WithNullKey_ThrowsException()
    {
        var state = new State();
        Assert.Throws<ArgumentNullException>(() => state.WithProperty(null!, "Value"));
    }

    [Fact]
    public void WithProperty_WithNullValue_HandlesNull()
    {
        var state = new State();
        var newState = state.WithProperty("Key", null!);

        Assert.True(newState.HasProperty("Key"));
        Assert.Null(newState.GetProperty<object>("Key"));
    }

    [Fact]
    public void WithProperties_WithEmptyDictionary_ReturnsEquivalentState()
    {
        var state = new State().WithProperty("Key", "Value");
        var updates = new Dictionary<string, object>();

        var newState = state.WithProperties(updates);

        // WithProperties creates a new instance, but state should be equivalent
        Assert.Equal(state.Properties, newState.Properties);
        Assert.True(StateComparer.AreEqual(state, newState));
    }
}


