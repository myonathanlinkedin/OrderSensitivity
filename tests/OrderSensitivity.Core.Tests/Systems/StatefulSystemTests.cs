using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Patterns;
using OrderSensitivity.Core.Systems;
using Xunit;

namespace OrderSensitivity.Core.Tests.Systems;

public class StatefulSystemTests
{
    private class TestOperation : OrderInsensitiveOperation
    {
        private readonly string _propertyName;
        public TestOperation(string propertyName) => _propertyName = propertyName;
        public override string Name => $"Test({_propertyName})";
        public override State Execute(State currentState) => currentState.WithProperty(_propertyName, true);
    }

    [Fact]
    public void Execute_SingleOperation_UpdatesState()
    {
        var system = new StatefulSystem(new State());
        var operation = new TestOperation("Key1");

        var result = system.Execute(operation);

        Assert.True(result.GetProperty<bool>("Key1", false));
        Assert.True(system.CurrentState.GetProperty<bool>("Key1", false));
    }

    [Fact]
    public void Execute_MultipleOperations_UpdatesStateSequentially()
    {
        var system = new StatefulSystem(new State());
        var op1 = new TestOperation("Key1");
        var op2 = new TestOperation("Key2");

        system.Execute(op1);
        var result = system.Execute(op2);

        Assert.True(result.GetProperty<bool>("Key1", false));
        Assert.True(result.GetProperty<bool>("Key2", false));
    }

    [Fact]
    public void ExecuteSequence_ValidSequence_ExecutesAllOperations()
    {
        var system = new StatefulSystem(new State());
        var operations = new IOperation[]
        {
            new TestOperation("Key1"),
            new TestOperation("Key2")
        };
        var sequence = new OperationSequence(operations);

        var result = system.ExecuteSequence(sequence);

        Assert.True(result.GetProperty<bool>("Key1", false));
        Assert.True(result.GetProperty<bool>("Key2", false));
    }

    [Fact]
    public void ExecuteSequence_InvalidSequence_ThrowsException()
    {
        var system = new StatefulSystem(new State());
        var operations = Array.Empty<IOperation>();
        var sequence = new OperationSequence(operations);

        Assert.Throws<InvalidOperationException>(() => system.ExecuteSequence(sequence));
    }

    [Fact]
    public void Reset_ResetsToInitialState()
    {
        var initialState = new State().WithProperty("Initial", true);
        var system = new StatefulSystem(initialState);
        system.Execute(new TestOperation("New"));

        system.Reset(initialState);

        Assert.True(system.CurrentState.GetProperty<bool>("Initial", false));
        Assert.False(system.CurrentState.HasProperty("New"));
    }

    [Fact]
    public void CurrentState_ReturnsCurrentState()
    {
        var initialState = new State().WithProperty("Key", "Value");
        var system = new StatefulSystem(initialState);

        Assert.Equal(initialState.Properties["Key"], system.CurrentState.Properties["Key"]);
    }

    [Fact]
    public void Execute_WithNullOperation_ThrowsException()
    {
        var system = new StatefulSystem(new State());

        Assert.Throws<ArgumentNullException>(() => system.Execute(null!));
    }

    [Fact]
    public void ExecuteSequence_WithNullSequence_ThrowsException()
    {
        var system = new StatefulSystem(new State());

        Assert.Throws<ArgumentNullException>(() => system.ExecuteSequence(null!));
    }

    [Fact]
    public void Reset_WithNullState_ThrowsException()
    {
        var system = new StatefulSystem(new State());

        Assert.Throws<ArgumentNullException>(() => system.Reset(null!));
    }
}

