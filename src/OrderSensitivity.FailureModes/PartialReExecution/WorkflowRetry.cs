using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Systems;
using OrderSensitivity.Core.Utilities;

namespace OrderSensitivity.FailureModes.PartialReExecution;

/// <summary>
/// Workflow retry mechanism that demonstrates partial re-execution issues.
/// </summary>
public class WorkflowRetry
{
    private readonly WorkflowSystem _workflow;
    private readonly Dictionary<string, State> _stepStates = new();

    public WorkflowRetry(WorkflowSystem workflow)
    {
        _workflow = workflow;
    }

    /// <summary>
    /// Executes a step and records its state.
    /// </summary>
    public State ExecuteStep(string stepName, State contextBefore)
    {
        var result = _workflow.ExecuteStep(stepName);
        _stepStates[stepName] = contextBefore;
        return result;
    }

    /// <summary>
    /// Retries a failed step.
    /// </summary>
    public State RetryStep(string stepName, State newContext)
    {
        // Retry with potentially different context
        _workflow.Reset(newContext);
        return _workflow.ExecuteStep(stepName);
    }

    /// <summary>
    /// Checks if retry produces different result.
    /// </summary>
    public bool HasRetryIssue(string stepName, State originalContext, State retryContext)
    {
        _workflow.Reset(originalContext);
        var originalResult = _workflow.ExecuteStep(stepName);

        _workflow.Reset(retryContext);
        var retryResult = _workflow.ExecuteStep(stepName);

        return !StateComparer.AreEqual(originalResult, retryResult);
    }
}


