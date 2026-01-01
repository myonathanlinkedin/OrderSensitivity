using System;
using OrderSensitivity.Core.Models;

namespace OrderSensitivity.Core.Systems;

/// <summary>
/// Represents a workflow step.
/// </summary>
public record WorkflowStep
{
    public string Name { get; init; } = string.Empty;
    public IOperation Operation { get; init; } = null!;
    public bool IsRequired { get; init; }
    public List<string> Dependencies { get; init; } = new();
}

/// <summary>
/// Workflow system that executes steps in a defined order.
/// </summary>
public class WorkflowSystem
{
    private readonly List<WorkflowStep> _steps = new();
    private State _currentState;
    private readonly Dictionary<string, bool> _completedSteps = new();

    public IReadOnlyList<WorkflowStep> Steps => _steps.AsReadOnly();
    public State CurrentState => _currentState;
    public IReadOnlyDictionary<string, bool> CompletedSteps => _completedSteps;

    public WorkflowSystem(State initialState)
    {
        _currentState = initialState;
    }

    public WorkflowSystem() : this(new State())
    {
    }

    /// <summary>
    /// Adds a step to the workflow.
    /// </summary>
    public void AddStep(WorkflowStep step)
    {
        if (step == null)
        {
            throw new ArgumentNullException(nameof(step));
        }

        _steps.Add(step);
        _completedSteps[step.Name] = false;
    }

    /// <summary>
    /// Executes a specific step.
    /// </summary>
    public State ExecuteStep(string stepName)
    {
        if (stepName == null)
        {
            throw new ArgumentNullException(nameof(stepName));
        }

        if (string.IsNullOrWhiteSpace(stepName))
        {
            throw new ArgumentException("Step name cannot be empty", nameof(stepName));
        }

        var step = _steps.FirstOrDefault(s => s.Name == stepName);
        if (step == null)
        {
            throw new InvalidOperationException($"Step {stepName} not found");
        }

        // Check dependencies
        foreach (var dependency in step.Dependencies)
        {
            if (!_completedSteps.TryGetValue(dependency, out var completed) || !completed)
            {
                throw new InvalidOperationException($"Step {stepName} requires {dependency} to be completed first");
            }
        }

        _currentState = step.Operation.Execute(_currentState);
        _completedSteps[stepName] = true;
        return _currentState;
    }

    /// <summary>
    /// Executes all steps in order.
    /// </summary>
    public State ExecuteAll()
    {
        var executionOrder = DetermineExecutionOrder();
        foreach (var stepName in executionOrder)
        {
            ExecuteStep(stepName);
        }
        return _currentState;
    }

    /// <summary>
    /// Determines the execution order based on dependencies.
    /// </summary>
    private List<string> DetermineExecutionOrder()
    {
        var order = new List<string>();
        var visited = new HashSet<string>();
        var visiting = new HashSet<string>();

        void Visit(string stepName)
        {
            if (visiting.Contains(stepName))
            {
                throw new InvalidOperationException($"Circular dependency detected involving {stepName}");
            }

            if (visited.Contains(stepName))
            {
                return;
            }

            visiting.Add(stepName);
            var step = _steps.First(s => s.Name == stepName);
            
            foreach (var dependency in step.Dependencies)
            {
                Visit(dependency);
            }

            visiting.Remove(stepName);
            visited.Add(stepName);
            order.Add(stepName);
        }

        foreach (var step in _steps)
        {
            if (!visited.Contains(step.Name))
            {
                Visit(step.Name);
            }
        }

        return order;
    }

    /// <summary>
    /// Resets the workflow to initial state.
    /// </summary>
    public void Reset(State initialState)
    {
        _currentState = initialState;
        foreach (var key in _completedSteps.Keys.ToList())
        {
            _completedSteps[key] = false;
        }
    }
}


