using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Patterns;

namespace OrderSensitivity.Examples.Workflow;

/// <summary>
/// Processes payment - must execute after validation.
/// This operation is order-sensitive because it depends on validation result.
/// </summary>
public class ProcessPaymentOperation : StateDependentOperation
{
    public override string Name => "ProcessPayment";

    protected override object? GetDependentValue(State currentState)
    {
        return WorkflowState.IsValid(currentState);
    }

    public override State Execute(State currentState)
    {
        var isValid = WorkflowState.IsValid(currentState);
        if (!isValid)
        {
            throw new InvalidOperationException("Cannot process payment: validation failed");
        }
        return currentState.WithProperty("PaymentProcessed", true);
    }

    public override OperationMetadata Metadata => base.Metadata with
    {
        Description = "Processes payment if validation passed"
    };
}


