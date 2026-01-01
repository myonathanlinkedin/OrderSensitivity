using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Patterns;
using OrderSensitivity.Core.Systems;
using Xunit;

namespace OrderSensitivity.Core.Tests.Systems;

public class WorkflowSystemTests
{
    private class TestOperation : OrderInsensitiveOperation
    {
        private readonly string _stepName;
        public TestOperation(string stepName) => _stepName = stepName;
        public override string Name => _stepName;
        public override State Execute(State currentState) => currentState.WithProperty(_stepName, true);
    }

    [Fact]
    public void AddStep_AddsStepToWorkflow()
    {
        var system = new WorkflowSystem(new State());
        var step = new WorkflowStep
        {
            Name = "Step1",
            Operation = new TestOperation("Step1")
        };

        system.AddStep(step);

        Assert.Single(system.Steps);
        Assert.Equal("Step1", system.Steps[0].Name);
    }

    [Fact]
    public void ExecuteStep_ExecutesStepOperation()
    {
        var system = new WorkflowSystem(new State());
        var step = new WorkflowStep
        {
            Name = "Step1",
            Operation = new TestOperation("Step1")
        };
        system.AddStep(step);

        var result = system.ExecuteStep("Step1");

        Assert.True(result.GetProperty<bool>("Step1", false));
        Assert.True(system.CompletedSteps["Step1"]);
    }

    [Fact]
    public void ExecuteStep_WithDependencies_ChecksDependencies()
    {
        var system = new WorkflowSystem(new State());
        var step1 = new WorkflowStep { Name = "Step1", Operation = new TestOperation("Step1") };
        var step2 = new WorkflowStep
        {
            Name = "Step2",
            Operation = new TestOperation("Step2"),
            Dependencies = new List<string> { "Step1" }
        };
        system.AddStep(step1);
        system.AddStep(step2);

        system.ExecuteStep("Step1");
        var result = system.ExecuteStep("Step2");

        Assert.True(result.GetProperty<bool>("Step2", false));
    }

    [Fact]
    public void ExecuteStep_WithoutDependencies_ThrowsException()
    {
        var system = new WorkflowSystem(new State());
        var step1 = new WorkflowStep { Name = "Step1", Operation = new TestOperation("Step1") };
        var step2 = new WorkflowStep
        {
            Name = "Step2",
            Operation = new TestOperation("Step2"),
            Dependencies = new List<string> { "Step1" }
        };
        system.AddStep(step1);
        system.AddStep(step2);

        Assert.Throws<InvalidOperationException>(() => system.ExecuteStep("Step2"));
    }

    [Fact]
    public void ExecuteStep_WithNonExistentStep_ThrowsException()
    {
        var system = new WorkflowSystem(new State());

        Assert.Throws<InvalidOperationException>(() => system.ExecuteStep("NonExistent"));
    }

    [Fact]
    public void ExecuteAll_ExecutesAllStepsInOrder()
    {
        var system = new WorkflowSystem(new State());
        var step1 = new WorkflowStep { Name = "Step1", Operation = new TestOperation("Step1") };
        var step2 = new WorkflowStep
        {
            Name = "Step2",
            Operation = new TestOperation("Step2"),
            Dependencies = new List<string> { "Step1" }
        };
        system.AddStep(step1);
        system.AddStep(step2);

        var result = system.ExecuteAll();

        Assert.True(result.GetProperty<bool>("Step1", false));
        Assert.True(result.GetProperty<bool>("Step2", false));
        Assert.True(system.CompletedSteps["Step1"]);
        Assert.True(system.CompletedSteps["Step2"]);
    }

    [Fact]
    public void ExecuteAll_WithCircularDependency_ThrowsException()
    {
        var system = new WorkflowSystem(new State());
        var step1 = new WorkflowStep
        {
            Name = "Step1",
            Operation = new TestOperation("Step1"),
            Dependencies = new List<string> { "Step2" }
        };
        var step2 = new WorkflowStep
        {
            Name = "Step2",
            Operation = new TestOperation("Step2"),
            Dependencies = new List<string> { "Step1" }
        };
        system.AddStep(step1);
        system.AddStep(step2);

        Assert.Throws<InvalidOperationException>(() => system.ExecuteAll());
    }

    [Fact]
    public void Reset_ResetsStateAndCompletedSteps()
    {
        var initialState = new State().WithProperty("Initial", true);
        var system = new WorkflowSystem(initialState);
        var step = new WorkflowStep { Name = "Step1", Operation = new TestOperation("Step1") };
        system.AddStep(step);
        system.ExecuteStep("Step1");

        system.Reset(initialState);

        Assert.True(system.CurrentState.GetProperty<bool>("Initial", false));
        Assert.False(system.CompletedSteps["Step1"]);
    }

    [Fact]
    public void AddStep_WithNullStep_ThrowsException()
    {
        var system = new WorkflowSystem(new State());

        Assert.Throws<ArgumentNullException>(() => system.AddStep(null!));
    }

    [Fact]
    public void ExecuteStep_WithNullStepName_ThrowsException()
    {
        var system = new WorkflowSystem(new State());

        Assert.Throws<ArgumentNullException>(() => system.ExecuteStep(null!));
    }

    [Fact]
    public void ExecuteStep_WithEmptyStepName_ThrowsException()
    {
        var system = new WorkflowSystem(new State());

        Assert.Throws<ArgumentException>(() => system.ExecuteStep(""));
    }
}

