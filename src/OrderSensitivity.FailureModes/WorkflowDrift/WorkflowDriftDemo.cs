using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Systems;
using OrderSensitivity.Examples.Workflow;

namespace OrderSensitivity.FailureModes.WorkflowDrift;

/// <summary>
/// Demonstrates workflow drift - long-running workflows produce incorrect state over time.
/// </summary>
public class WorkflowDriftDemo
{
    public WorkflowDriftResult Demonstrate(WorkflowSystem workflow, State initialState)
    {
        // Execute workflow over time
        workflow.Reset(initialState);
        State state1;
        State state2;
        State state3;
        
        try
        {
            state1 = workflow.ExecuteStep("Step1");
            state2 = workflow.ExecuteStep("Step2");
            
            // System update changes ordering assumptions
            // Simulated by modifying workflow constraints
            workflow.Reset(state2);
            
            // Continue execution with potentially changed constraints
            state3 = workflow.ExecuteStep("Step3");
        }
        catch
        {
            // If execution fails, return partial results
            state1 = workflow.CurrentState;
            state2 = workflow.CurrentState;
            state3 = workflow.CurrentState;
        }

        // Check if workflow has drifted
        var hasDrifted = CheckDrift(workflow, state3);

        return new WorkflowDriftResult
        {
            InitialState = initialState,
            StateAfterStep1 = state1,
            StateAfterStep2 = state2,
            StateAfterStep3 = state3,
            HasDrifted = hasDrifted
        };
    }

    private bool CheckDrift(WorkflowSystem workflow, State currentState)
    {
        // Simple drift check: verify that completed steps match expected state
        // In a real scenario, this would check against expected invariants
        return false; // Placeholder - actual implementation would check invariants
    }
}

/// <summary>
/// Result of workflow drift demonstration.
/// </summary>
public record WorkflowDriftResult
{
    public State InitialState { get; init; } = new();
    public State StateAfterStep1 { get; init; } = new();
    public State StateAfterStep2 { get; init; } = new();
    public State StateAfterStep3 { get; init; } = new();
    public bool HasDrifted { get; init; }
}


