using OrderSensitivity.Core.Models;
using OrderSensitivity.Examples.Workflow;
using Xunit;

namespace OrderSensitivity.Examples.Tests;

public class WorkflowTests
{
    [Fact]
    public void ValidateInput_Then_ProcessPayment_ExecutesSuccessfully()
    {
        var initialState = WorkflowState.Create("payment_data");
        var validate = new ValidateInputOperation();
        var processPayment = new ProcessPaymentOperation();

        var state1 = validate.Execute(initialState);
        var state2 = processPayment.Execute(state1);

        Assert.True(WorkflowState.IsValid(state1));
        Assert.True(WorkflowState.IsPaymentProcessed(state2));
    }

    [Fact]
    public void ProcessPayment_Without_Validation_ThrowsException()
    {
        var initialState = WorkflowState.Create("payment_data");
        var processPayment = new ProcessPaymentOperation();

        Assert.Throws<InvalidOperationException>(() => processPayment.Execute(initialState));
    }

    [Fact]
    public void SendNotification_Without_Payment_ThrowsException()
    {
        var initialState = WorkflowState.Create("payment_data");
        var sendNotification = new SendNotificationOperation();

        Assert.Throws<InvalidOperationException>(() => sendNotification.Execute(initialState));
    }

    [Fact]
    public void CompleteWorkflow_ExecutesInCorrectOrder()
    {
        var initialState = WorkflowState.Create("payment_data");
        var validate = new ValidateInputOperation();
        var processPayment = new ProcessPaymentOperation();
        var sendNotification = new SendNotificationOperation();

        var state1 = validate.Execute(initialState);
        var state2 = processPayment.Execute(state1);
        var state3 = sendNotification.Execute(state2);

        Assert.True(WorkflowState.IsValid(state1));
        Assert.True(WorkflowState.IsPaymentProcessed(state2));
        Assert.True(WorkflowState.IsNotificationSent(state3));
    }
}

