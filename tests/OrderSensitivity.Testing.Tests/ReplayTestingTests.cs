using OrderSensitivity.Core.Models;
using OrderSensitivity.Examples.UserAccount;
using OrderSensitivity.Testing.ReplayTesting;
using Xunit;

namespace OrderSensitivity.Testing.Tests;

public class ReplayTestingTests
{
    [Fact]
    public void ReplayTestRunner_WithSameSequence_NoDivergence()
    {
        var initialState = AccountState.Create(0m);
        var operations = new IOperation[]
        {
            new DepositOperation(100m),
            new DepositOperation(50m)
        };
        var sequence = new OperationSequence(operations);
        var runner = new ReplayTestRunner();

        var result = runner.TestReplay(sequence, initialState);

        // Order-insensitive operations should not diverge
        Assert.False(result.HasDivergence);
    }

    [Fact]
    public void ReplayTestRecorder_RecordsOperations()
    {
        var initialState = AccountState.Create(0m);
        var operations = new IOperation[]
        {
            new DepositOperation(100m)
        };
        var sequence = new OperationSequence(operations);
        var recorder = new ReplayTestRecorder();

        recorder.RecordSequence(sequence, initialState, sequence.Execute(initialState));

        var recorded = recorder.GetRecordedSequence();
        Assert.Single(recorded.Operations);
    }
}

