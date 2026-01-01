using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Patterns;
using OrderSensitivity.Core.Systems;
using OrderSensitivity.Examples.UserAccount;
using static OrderSensitivity.Examples.UserAccount.AccountState;
using Xunit;

namespace OrderSensitivity.Core.Tests.Systems;

public class EventSourcingSystemTests
{
    private class TestOperation : OrderInsensitiveOperation
    {
        private readonly string _value;
        public TestOperation(string value) => _value = value;
        public override string Name => $"Test({_value})";
        public override State Execute(State currentState) => currentState.WithProperty("Value", _value);
    }

    [Fact]
    public void AppendEvent_AddsEventToLog()
    {
        var system = new EventSourcingSystem(new State());
        var evt = new Event { Type = "Test", Data = new Dictionary<string, object> { ["Value"] = "TestValue" } };
        var operation = new TestOperation("TestValue");

        system.AppendEvent(evt, operation);

        Assert.Single(system.EventLog);
        Assert.Equal("Test", system.EventLog[0].Type);
    }

    [Fact]
    public void AppendEvent_UpdatesCurrentState()
    {
        var system = new EventSourcingSystem(new State());
        var evt = new Event { Type = "Test", Data = new Dictionary<string, object> { ["Value"] = "TestValue" } };
        var operation = new TestOperation("TestValue");

        system.AppendEvent(evt, operation);

        Assert.Equal("TestValue", system.CurrentState.GetProperty<string>("Value"));
    }

    [Fact]
    public void AppendEvent_AssignsSequenceNumbers()
    {
        var system = new EventSourcingSystem(new State());
        var evt1 = new Event { Type = "Test1" };
        var evt2 = new Event { Type = "Test2" };
        var op1 = new TestOperation("Value1");
        var op2 = new TestOperation("Value2");

        system.AppendEvent(evt1, op1);
        system.AppendEvent(evt2, op2);

        Assert.Equal(0, system.EventLog[0].SequenceNumber);
        Assert.Equal(1, system.EventLog[1].SequenceNumber);
    }

    [Fact]
    public void ReplayEvents_ReconstructsState()
    {
        var system = new EventSourcingSystem(new State());
        var events = new[]
        {
            new Event { Type = "Test1", SequenceNumber = 0, Data = new Dictionary<string, object> { ["Value"] = "Value1" } },
            new Event { Type = "Test2", SequenceNumber = 1, Data = new Dictionary<string, object> { ["Value"] = "Value2" } }
        };
        Func<Event, IOperation> eventToOperation = evt => new TestOperation(evt.Data["Value"].ToString()!);

        var result = system.ReplayEvents(events, eventToOperation);

        Assert.Equal("Value2", result.GetProperty<string>("Value"));
    }

    [Fact]
    public void ReplayAll_ReplaysAllEvents()
    {
        var system = new EventSourcingSystem(new State());
        var evt1 = new Event { Type = "Test1", Data = new Dictionary<string, object> { ["Value"] = "Value1" } };
        var evt2 = new Event { Type = "Test2", Data = new Dictionary<string, object> { ["Value"] = "Value2" } };
        var op1 = new TestOperation("Value1");
        var op2 = new TestOperation("Value2");

        system.AppendEvent(evt1, op1);
        system.AppendEvent(evt2, op2);

        Func<Event, IOperation> eventToOperation = evt => new TestOperation(evt.Data["Value"].ToString()!);
        var replayedState = system.ReplayAll(eventToOperation);

        Assert.Equal("Value2", replayedState.GetProperty<string>("Value"));
    }

    [Fact]
    public void ReplayInDifferentOrder_WithOrderSensitiveOperations_ProducesDifferentState()
    {
        var system = new EventSourcingSystem(new State());
        var events = new[]
        {
            new Event { Type = "Deposit", SequenceNumber = 0, Timestamp = DateTime.UtcNow.AddSeconds(1), Data = new Dictionary<string, object> { ["Amount"] = 100m } },
            new Event { Type = "ApplyFee", SequenceNumber = 1, Timestamp = DateTime.UtcNow.AddSeconds(2), Data = new Dictionary<string, object> { ["FeePercentage"] = 0.1m } }
        };
        Func<Event, IOperation> eventToOperation = evt => evt.Type switch
        {
            "Deposit" => new DepositOperation(Convert.ToDecimal(evt.Data["Amount"])),
            "ApplyFee" => new ApplyFeeOperation(Convert.ToDecimal(evt.Data["FeePercentage"])),
            _ => throw new InvalidOperationException()
        };

        // Replay in correct order (by sequence number)
        var state1 = system.ReplayEvents(events.OrderBy(e => e.SequenceNumber), eventToOperation);
        
        // Create events with swapped sequence numbers to simulate wrong order
        var wrongOrderEvents = new[]
        {
            new Event { Type = "ApplyFee", SequenceNumber = 0, Timestamp = DateTime.UtcNow.AddSeconds(2), Data = new Dictionary<string, object> { ["FeePercentage"] = 0.1m } },
            new Event { Type = "Deposit", SequenceNumber = 1, Timestamp = DateTime.UtcNow.AddSeconds(1), Data = new Dictionary<string, object> { ["Amount"] = 100m } }
        };
        var state2 = system.ReplayEvents(wrongOrderEvents.OrderBy(e => e.SequenceNumber), eventToOperation);

        // States should differ because order-sensitive operations are applied in different order
        var balance1 = AccountState.GetBalance(state1);
        var balance2 = AccountState.GetBalance(state2);
        
        // Deposit(100) then ApplyFee(10%) = 90
        // ApplyFee(10%) then Deposit(100) = 100
        Assert.NotEqual(balance1, balance2);
        Assert.Equal(90m, balance1); // Correct order
        Assert.Equal(100m, balance2); // Wrong order
    }

    [Fact]
    public void AppendEvent_WithNullEvent_ThrowsException()
    {
        var system = new EventSourcingSystem(new State());
        var operation = new TestOperation("Value");

        Assert.Throws<ArgumentNullException>(() => system.AppendEvent(null!, operation));
    }

    [Fact]
    public void AppendEvent_WithNullOperation_ThrowsException()
    {
        var system = new EventSourcingSystem(new State());
        var evt = new Event { Type = "Test", Data = new Dictionary<string, object>() };

        Assert.Throws<ArgumentNullException>(() => system.AppendEvent(evt, null!));
    }

    [Fact]
    public void ReplayEvents_WithNullEvents_ThrowsException()
    {
        var system = new EventSourcingSystem(new State());
        Func<Event, IOperation> eventToOperation = evt => new TestOperation("Value");

        Assert.Throws<ArgumentNullException>(() => system.ReplayEvents(null!, eventToOperation));
    }

    [Fact]
    public void ReplayEvents_WithEmptyEvents_ReturnsInitialState()
    {
        var system = new EventSourcingSystem(new State());
        var events = Array.Empty<Event>();
        Func<Event, IOperation> eventToOperation = evt => new TestOperation("Value");

        var result = system.ReplayEvents(events, eventToOperation);

        Assert.NotNull(result);
        Assert.Empty(result.Properties);
    }
}

