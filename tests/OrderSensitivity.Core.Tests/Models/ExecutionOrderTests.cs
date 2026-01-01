using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Patterns;
using Xunit;

namespace OrderSensitivity.Core.Tests.Models;

public class ExecutionOrderTests
{
    private class TestOperation : OrderInsensitiveOperation
    {
        private readonly string _name;
        public TestOperation(string name) => _name = name;
        public override string Name => _name;
        public override State Execute(State currentState) => currentState;
    }

    [Fact]
    public void IsValid_WithValidSequenceNumbers_ReturnsTrue()
    {
        var sequenceNumbers = new[] { 0, 1, 2 };
        var order = new ExecutionOrder(sequenceNumbers);

        Assert.True(order.IsValid());
    }

    [Fact]
    public void IsValid_WithDuplicateSequenceNumbers_ReturnsFalse()
    {
        var sequenceNumbers = new[] { 0, 1, 1 };
        var order = new ExecutionOrder(sequenceNumbers);

        Assert.False(order.IsValid());
    }

    [Fact]
    public void IsValid_WithNegativeSequenceNumbers_ReturnsFalse()
    {
        var sequenceNumbers = new[] { -1, 0, 1 };
        var order = new ExecutionOrder(sequenceNumbers);

        Assert.False(order.IsValid());
    }

    [Fact]
    public void GetPosition_WithExistingOperation_ReturnsPosition()
    {
        var operations = new IOperation[]
        {
            new TestOperation("Op1"),
            new TestOperation("Op2")
        };
        var order = new ExecutionOrder(operations);

        var position = order.GetPosition("Op1");

        Assert.Equal(0, position);
    }

    [Fact]
    public void GetPosition_WithNonExistentOperation_ReturnsNull()
    {
        var operations = new IOperation[]
        {
            new TestOperation("Op1")
        };
        var order = new ExecutionOrder(operations);

        var position = order.GetPosition("NonExistent");

        Assert.Null(position);
    }

    [Fact]
    public void ViolatesConstraints_WithValidPosition_ReturnsFalse()
    {
        var operations = new IOperation[]
        {
            new TestOperation("Op1")
        };
        var order = new ExecutionOrder(operations);

        var violates = order.ViolatesConstraints(operations[0], 0);

        Assert.False(violates);
    }

    [Fact]
    public void ViolatesConstraints_WithNegativePosition_ReturnsTrue()
    {
        var operations = new IOperation[]
        {
            new TestOperation("Op1")
        };
        var order = new ExecutionOrder(operations);

        var violates = order.ViolatesConstraints(operations[0], -1);

        Assert.True(violates);
    }

    [Fact]
    public void ViolatesConstraints_WithOutOfBoundsPosition_ReturnsTrue()
    {
        var operations = new IOperation[]
        {
            new TestOperation("Op1")
        };
        var order = new ExecutionOrder(operations);

        var violates = order.ViolatesConstraints(operations[0], 10);

        Assert.True(violates);
    }
}

