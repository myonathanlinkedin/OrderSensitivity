using OrderSensitivity.Core.Models;
using OrderSensitivity.Examples.Configuration;
using Xunit;

namespace OrderSensitivity.Examples.Tests;

public class ConfigurationTests
{
    [Fact]
    public void SetDefault_Then_Override_OverrideWins()
    {
        var initialState = ConfigState.Create();
        var setDefault = new SetDefaultOperation("A");
        var overrideOp = new OverrideOperation("key", "B");

        var state1 = setDefault.Execute(initialState);
        var state2 = overrideOp.Execute(state1);

        Assert.Equal("B", ConfigState.GetValue(state2, "key"));
    }

    [Fact]
    public void Override_Then_SetDefault_DefaultOverwrites()
    {
        var initialState = ConfigState.Create();
        var overrideOp = new OverrideOperation("key", "B");
        var setDefault = new SetDefaultOperation("A");

        var state1 = overrideOp.Execute(initialState);
        var state2 = setDefault.Execute(state1);

        Assert.Equal("A", ConfigState.GetValue(state2, "key"));
    }

    [Fact]
    public void SetDefault_And_Override_AreOrderSensitive()
    {
        var initialState = ConfigState.Create();
        var setDefault = new SetDefaultOperation("A");
        var overrideOp = new OverrideOperation("key", "B");

        var sequence1 = new OperationSequence(new IOperation[] { setDefault, overrideOp });
        var sequence2 = new OperationSequence(new IOperation[] { overrideOp, setDefault });

        var result1 = sequence1.Execute(initialState);
        var result2 = sequence2.Execute(initialState);

        Assert.NotEqual(ConfigState.GetValue(result1, "key"), ConfigState.GetValue(result2, "key"));
    }
}

