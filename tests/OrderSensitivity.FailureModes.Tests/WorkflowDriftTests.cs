using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Systems;
using OrderSensitivity.Examples.Workflow;
using OrderSensitivity.FailureModes.WorkflowDrift;
using Xunit;

namespace OrderSensitivity.FailureModes.Tests;

public class WorkflowDriftTests
{
    [Fact]
    public void WorkflowDrift_CanBeDemonstrated()
    {
        var workflow = new WorkflowSystem(WorkflowState.Create("payment_data"));
        var initialState = WorkflowState.Create("payment_data");
        
        // Add steps to workflow
        var step1 = new WorkflowStep
        {
            Name = "Step1",
            Operation = new ValidateInputOperation()
        };
        var step2 = new WorkflowStep
        {
            Name = "Step2",
            Operation = new ProcessPaymentOperation(),
            Dependencies = new List<string> { "Step1" }
        };
        var step3 = new WorkflowStep
        {
            Name = "Step3",
            Operation = new SendNotificationOperation(),
            Dependencies = new List<string> { "Step2" }
        };
        
        workflow.AddStep(step1);
        workflow.AddStep(step2);
        workflow.AddStep(step3);
        
        var demo = new WorkflowDriftDemo();
        var result = demo.Demonstrate(workflow, initialState);

        // Workflow drift demonstration should complete
        Assert.NotNull(result);
    }
}

