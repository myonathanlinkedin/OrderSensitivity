using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Patterns;
using OrderSensitivity.Core.Utilities;
using Xunit;

namespace OrderSensitivity.Core.Tests.Utilities;

public class OrderValidatorTests
{
    private class TestOperation : OrderInsensitiveOperation
    {
        private readonly string _name;
        public TestOperation(string name) => _name = name;
        public override string Name => _name;
        public override State Execute(State currentState) => currentState.WithProperty(_name, true);
    }

    [Fact]
    public void CheckSequence_WithValidConstraints_ReturnsValid()
    {
        var operations = new IOperation[]
        {
            new TestOperation("Op1"),
            new TestOperation("Op2")
        };
        var sequence = new OperationSequence(operations);
        var constraints = new[]
        {
            new OrderingConstraint
            {
                OperationName = "Op1",
                MinPosition = 0,
                MaxPosition = 0
            }
        };

        var result = OrderValidator.CheckSequence(sequence, constraints);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void CheckSequence_WithInvalidMinPosition_ReturnsInvalid()
    {
        var operations = new IOperation[]
        {
            new TestOperation("Op1"),
            new TestOperation("Op2")
        };
        var sequence = new OperationSequence(operations);
        var constraints = new[]
        {
            new OrderingConstraint
            {
                OperationName = "Op1",
                MinPosition = 1
            }
        };

        var result = OrderValidator.CheckSequence(sequence, constraints);

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void CheckSequence_WithInvalidMaxPosition_ReturnsInvalid()
    {
        var operations = new IOperation[]
        {
            new TestOperation("Op1"),
            new TestOperation("Op2")
        };
        var sequence = new OperationSequence(operations);
        var constraints = new[]
        {
            new OrderingConstraint
            {
                OperationName = "Op2",
                MaxPosition = 0
            }
        };

        var result = OrderValidator.CheckSequence(sequence, constraints);

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void CheckSequence_WithPrecedenceViolation_ReturnsInvalid()
    {
        var operations = new IOperation[]
        {
            new TestOperation("Op2"),
            new TestOperation("Op1")
        };
        var sequence = new OperationSequence(operations);
        var constraints = new[]
        {
            new OrderingConstraint
            {
                OperationName = "Op1",
                MustPrecede = new List<string> { "Op2" }
            }
        };

        var result = OrderValidator.CheckSequence(sequence, constraints);

        // Op1 is at position 1, Op2 is at position 0
        // Op1 must precede Op2, but Op2 comes first - violation
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void CheckSequence_WithFollowViolation_ReturnsInvalid()
    {
        var operations = new IOperation[]
        {
            new TestOperation("Op1"),
            new TestOperation("Op2")
        };
        var sequence = new OperationSequence(operations);
        var constraints = new[]
        {
            new OrderingConstraint
            {
                OperationName = "Op1",
                MustFollow = new List<string> { "Op2" }
            }
        };

        var result = OrderValidator.CheckSequence(sequence, constraints);

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void CheckSequence_WithMissingPrecedenceOperation_ReturnsWarning()
    {
        var operations = new IOperation[]
        {
            new TestOperation("Op1")
        };
        var sequence = new OperationSequence(operations);
        var constraints = new[]
        {
            new OrderingConstraint
            {
                OperationName = "Op1",
                MustPrecede = new List<string> { "Op2" }
            }
        };

        var result = OrderValidator.CheckSequence(sequence, constraints);

        Assert.True(result.IsValid);
        Assert.NotEmpty(result.Warnings);
    }

    [Fact]
    public void IsOrderSensitive_WithOrderSensitiveOperation_ReturnsTrue()
    {
        var operations = new IOperation[]
        {
            new TestOrderSensitiveOperation(),
            new TestOperation("Op2")
        };
        var initialState = new State();

        var result = OrderValidator.IsOrderSensitive(operations, initialState);

        Assert.True(result);
    }

    [Fact]
    public void IsOrderSensitive_WithOrderInsensitiveOperations_ReturnsFalse()
    {
        var operations = new IOperation[]
        {
            new TestOperation("Op1"),
            new TestOperation("Op2")
        };
        var initialState = new State();

        var result = OrderValidator.IsOrderSensitive(operations, initialState);

        Assert.False(result);
    }

    [Fact]
    public void IsOrderSensitive_WithLessThanTwoOperations_ReturnsFalse()
    {
        var operations = new IOperation[]
        {
            new TestOperation("Op1")
        };
        var initialState = new State();

        var result = OrderValidator.IsOrderSensitive(operations, initialState);

        Assert.False(result);
    }

    [Fact]
    public void CheckSequence_WithNullSequence_ThrowsException()
    {
        var constraints = Array.Empty<OrderingConstraint>();

        Assert.Throws<ArgumentNullException>(() => OrderValidator.CheckSequence(null!, constraints));
    }

    [Fact]
    public void CheckSequence_WithNullConstraints_HandlesNull()
    {
        var operations = new IOperation[]
        {
            new TestOperation("Op1")
        };
        var sequence = new OperationSequence(operations);

        var result = OrderValidator.CheckSequence(sequence, null!);

        // Should handle null constraints gracefully
        Assert.True(result.IsValid);
    }

    [Fact]
    public void IsOrderSensitive_WithNullOperations_ThrowsException()
    {
        var initialState = new State();

        Assert.Throws<ArgumentNullException>(() => OrderValidator.IsOrderSensitive(null!, initialState));
    }

    private class TestOrderSensitiveOperation : OrderSensitiveOperation
    {
        public override string Name => "TestOrderSensitive";
        public override State Execute(State currentState) => currentState.WithProperty("Test", true);
    }
}

