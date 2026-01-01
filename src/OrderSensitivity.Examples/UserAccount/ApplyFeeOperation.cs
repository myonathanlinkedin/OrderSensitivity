using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Patterns;

namespace OrderSensitivity.Examples.UserAccount;

/// <summary>
/// Apply fee operation - applies a percentage fee to current balance.
/// This operation is order-sensitive because it depends on the current balance.
/// </summary>
public class ApplyFeeOperation : StateDependentOperation
{
    private readonly decimal _feePercentage;

    public ApplyFeeOperation(decimal feePercentage)
    {
        _feePercentage = feePercentage;
    }

    public override string Name => $"ApplyFee({_feePercentage:P})";

    protected override object? GetDependentValue(State currentState)
    {
        return AccountState.GetBalance(currentState);
    }

    public override State Execute(State currentState)
    {
        var balance = AccountState.GetBalance(currentState);
        var fee = balance * _feePercentage;
        var newBalance = balance - fee;
        return currentState.WithProperty("Balance", newBalance);
    }

    public override OperationMetadata Metadata => base.Metadata with
    {
        Description = $"Applies {_feePercentage:P} fee to current balance",
        Parameters = new Dictionary<string, object> { ["FeePercentage"] = _feePercentage }
    };
}


