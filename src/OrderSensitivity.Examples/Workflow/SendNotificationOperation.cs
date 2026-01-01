using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Patterns;

namespace OrderSensitivity.Examples.Workflow;

/// <summary>
/// Sends notification - should execute after payment processing.
/// This operation is order-sensitive because it depends on payment status.
/// </summary>
public class SendNotificationOperation : StateDependentOperation
{
    public override string Name => "SendNotification";

    protected override object? GetDependentValue(State currentState)
    {
        return WorkflowState.IsPaymentProcessed(currentState);
    }

    public override State Execute(State currentState)
    {
        var paymentProcessed = WorkflowState.IsPaymentProcessed(currentState);
        if (!paymentProcessed)
        {
            throw new InvalidOperationException("Cannot send notification: payment not processed");
        }
        return currentState.WithProperty("NotificationSent", true);
    }

    public override OperationMetadata Metadata => base.Metadata with
    {
        Description = "Sends notification after payment processing"
    };
}


