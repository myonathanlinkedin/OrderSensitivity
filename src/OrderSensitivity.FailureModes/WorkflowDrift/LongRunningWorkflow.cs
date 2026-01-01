using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Systems;

namespace OrderSensitivity.FailureModes.WorkflowDrift;

/// <summary>
/// Long-running workflow that can experience drift over time.
/// </summary>
public class LongRunningWorkflow
{
    private readonly WorkflowSystem _workflow;
    private readonly Dictionary<string, DateTime> _stepTimestamps = new();
    private bool _constraintsUpdated = false;

    public LongRunningWorkflow(WorkflowSystem workflow)
    {
        _workflow = workflow;
    }

    /// <summary>
    /// Executes a step and records timestamp.
    /// </summary>
    public State ExecuteStep(string stepName, State currentState)
    {
        _workflow.Reset(currentState);
        var result = _workflow.ExecuteStep(stepName);
        _stepTimestamps[stepName] = DateTime.UtcNow;
        return result;
    }

    /// <summary>
    /// Simulates system update that changes ordering constraints.
    /// </summary>
    public void UpdateOrderingConstraints()
    {
        _constraintsUpdated = true;
        // In a real scenario, this would modify workflow step dependencies
    }

    /// <summary>
    /// Checks if workflow has drifted from expected behavior.
    /// </summary>
    public bool HasDrifted(State currentState)
    {
        // Check if constraints were updated during execution
        if (_constraintsUpdated)
        {
            // Workflow may have drifted if constraints changed mid-execution
            return true;
        }

        // Additional drift checks would go here
        return false;
    }

    /// <summary>
    /// Gets the execution timeline.
    /// </summary>
    public Dictionary<string, DateTime> GetExecutionTimeline()
    {
        return new Dictionary<string, DateTime>(_stepTimestamps);
    }
}


