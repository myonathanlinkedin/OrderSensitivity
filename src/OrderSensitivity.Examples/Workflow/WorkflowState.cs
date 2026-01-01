using OrderSensitivity.Core.Models;

namespace OrderSensitivity.Examples.Workflow;

/// <summary>
/// State for a workflow system.
/// </summary>
public static class WorkflowState
{
    public static State Create(string input)
    {
        return new State()
            .WithProperty("Input", input)
            .WithProperty("IsValid", false)
            .WithProperty("PaymentProcessed", false)
            .WithProperty("NotificationSent", false);
    }

    public static string GetInput(State state)
    {
        return state.GetProperty<string>("Input", string.Empty) ?? string.Empty;
    }

    public static bool IsValid(State state)
    {
        return state.GetProperty<bool>("IsValid", false);
    }

    public static bool IsPaymentProcessed(State state)
    {
        return state.GetProperty<bool>("PaymentProcessed", false);
    }

    public static bool IsNotificationSent(State state)
    {
        return state.GetProperty<bool>("NotificationSent", false);
    }
}


