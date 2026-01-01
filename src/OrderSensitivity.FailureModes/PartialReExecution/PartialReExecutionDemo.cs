using OrderSensitivity.Core.Models;
using OrderSensitivity.Examples.Workflow;

namespace OrderSensitivity.FailureModes.PartialReExecution;

/// <summary>
/// Demonstrates partial re-execution - retry sees different state than original execution.
/// </summary>
public class PartialReExecutionDemo
{
    public PartialReExecutionResult Demonstrate()
    {
        // Original execution context
        var context1 = WorkflowState.Create("payment_data");
        context1 = context1.WithProperty("Step1Complete", true);
        
        var step2 = new ProcessPaymentOperation();
        
        // This will fail because validation hasn't run
        State? originalResult = null;
        Exception? originalException = null;
        try
        {
            originalResult = step2.Execute(context1);
        }
        catch (Exception ex)
        {
            originalException = ex;
        }

        // Retry after other steps executed
        var context2 = WorkflowState.Create("payment_data");
        context2 = context2.WithProperty("Step1Complete", true);
        context2 = context2.WithProperty("Step3Complete", true);  // Additional state change
        context2 = context2.WithProperty("Data", "modified_data");  // Data changed

        State? retryResult = null;
        Exception? retryException = null;
        try
        {
            retryResult = step2.Execute(context2);
        }
        catch (Exception ex)
        {
            retryException = ex;
        }

        // Results differ - partial re-execution issue
        var hasIssue = originalResult != retryResult || 
                      (originalException != null && retryException == null) ||
                      (originalException == null && retryException != null);

        return new PartialReExecutionResult
        {
            OriginalContext = context1,
            RetryContext = context2,
            OriginalResult = originalResult,
            RetryResult = retryResult,
            OriginalException = originalException,
            RetryException = retryException,
            HasIssue = hasIssue
        };
    }
}

/// <summary>
/// Result of partial re-execution demonstration.
/// </summary>
public record PartialReExecutionResult
{
    public State OriginalContext { get; init; } = new();
    public State RetryContext { get; init; } = new();
    public State? OriginalResult { get; init; }
    public State? RetryResult { get; init; }
    public Exception? OriginalException { get; init; }
    public Exception? RetryException { get; init; }
    public bool HasIssue { get; init; }
}


