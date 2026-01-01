using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Patterns;

namespace OrderSensitivity.Examples.Workflow;

/// <summary>
/// Validates input - must execute before payment processing.
/// This operation is order-sensitive because payment depends on validation.
/// </summary>
public class ValidateInputOperation : OrderSensitiveOperation
{
    public override string Name => "ValidateInput";

    public override State Execute(State currentState)
    {
        var input = WorkflowState.GetInput(currentState);
        var isValid = !string.IsNullOrWhiteSpace(input) && input.Length > 0;
        return currentState.WithProperty("IsValid", isValid);
    }

    public override OperationMetadata Metadata => base.Metadata with
    {
        Description = "Validates input data"
    };
}


