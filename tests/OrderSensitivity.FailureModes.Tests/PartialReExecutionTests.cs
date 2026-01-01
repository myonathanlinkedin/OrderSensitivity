using OrderSensitivity.Examples.UserAccount;
using OrderSensitivity.FailureModes.PartialReExecution;
using Xunit;

namespace OrderSensitivity.FailureModes.Tests;

public class PartialReExecutionTests
{
    [Fact]
    public void PartialReExecution_WithDifferentContext_ShowsIssue()
    {
        var demo = new PartialReExecutionDemo();
        var result = demo.Demonstrate();

        // Both should throw exception, but contexts are different
        // The issue is that retry sees different context than original
        Assert.NotNull(result.OriginalException);
        Assert.NotNull(result.RetryException);
        // Contexts differ even though both fail
        Assert.NotEqual(result.OriginalContext, result.RetryContext);
    }

    [Fact]
    public void PartialReExecution_OriginalContext_ThrowsException()
    {
        var demo = new PartialReExecutionDemo();
        var result = demo.Demonstrate();

        // Original execution should fail because validation hasn't run
        Assert.NotNull(result.OriginalException);
    }
}

