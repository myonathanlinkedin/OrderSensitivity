using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Patterns;

namespace OrderSensitivity.Examples.UserAccount;

/// <summary>
/// Withdraw operation - subtracts amount from account balance.
/// This operation is order-sensitive when combined with fee operations.
/// </summary>
public class WithdrawOperation : StateDependentOperation
{
    private readonly decimal _amount;

    public WithdrawOperation(decimal amount)
    {
        _amount = amount;
    }

    public override string Name => $"Withdraw({_amount})";

    protected override object? GetDependentValue(State currentState)
    {
        return AccountState.GetBalance(currentState);
    }

    public override State Execute(State currentState)
    {
        var balance = AccountState.GetBalance(currentState);
        if (balance < _amount)
        {
            throw new InvalidOperationException($"Insufficient balance. Current: {balance}, Requested: {_amount}");
        }
        var newBalance = balance - _amount;
        return currentState.WithProperty("Balance", newBalance);
    }

    public override OperationMetadata Metadata => base.Metadata with
    {
        Description = $"Withdraws {_amount} from account",
        Parameters = new Dictionary<string, object> { ["Amount"] = _amount }
    };
}


