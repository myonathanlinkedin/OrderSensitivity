using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Patterns;
using Xunit;

namespace OrderSensitivity.Core.Tests.Models;

public class OperationSequenceTests
{
    private class TestOperation : OrderInsensitiveOperation
    {
        private readonly string _name;
        private readonly string _propertyKey;
        private readonly object _propertyValue;

        public TestOperation(string name, string propertyKey, object propertyValue)
        {
            _name = name;
            _propertyKey = propertyKey;
            _propertyValue = propertyValue;
        }

        public override string Name => _name;

        public override State Execute(State currentState)
        {
            return currentState.WithProperty(_propertyKey, _propertyValue);
        }
    }

    [Fact]
    public void Execute_AppliesOperationsInOrder()
    {
        var operations = new IOperation[]
        {
            new TestOperation("Op1", "Key1", "Value1"),
            new TestOperation("Op2", "Key2", "Value2")
        };

        var sequence = new OperationSequence(operations);
        var initialState = new State();
        var finalState = sequence.Execute(initialState);

        Assert.Equal("Value1", finalState.GetProperty<string>("Key1"));
        Assert.Equal("Value2", finalState.GetProperty<string>("Key2"));
    }

    [Fact]
    public void IsValid_ReturnsTrueForValidSequence()
    {
        var operations = new IOperation[]
        {
            new TestOperation("Op1", "Key1", "Value1")
        };

        var sequence = new OperationSequence(operations);
        Assert.True(sequence.IsValid());
    }

    [Fact]
    public void Execute_WithEmptySequence_ThrowsException()
    {
        var operations = Array.Empty<IOperation>();
        var sequence = new OperationSequence(operations);
        var initialState = new State();

        Assert.Throws<InvalidOperationException>(() => sequence.Execute(initialState));
    }

    [Fact]
    public void Execute_WithNullOperations_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new OperationSequence(null!));
    }

    [Fact]
    public void IsValid_WithEmptySequence_ReturnsFalse()
    {
        var operations = Array.Empty<IOperation>();
        var sequence = new OperationSequence(operations);

        Assert.False(sequence.IsValid());
    }
}


