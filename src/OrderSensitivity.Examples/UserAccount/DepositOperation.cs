using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Patterns;

namespace OrderSensitivity.Examples.UserAccount;

/// <summary>
/// Deposit operation - adds amount to account balance.
/// This operation is order-insensitive when combined with other deposits.
/// </summary>
public class DepositOperation : OrderInsensitiveOperation
{
    private readonly decimal _amount;

    public DepositOperation(decimal amount)
    {
        _amount = amount;
    }

    public override string Name => $"Deposit({_amount})";

    public override State Execute(State currentState)
    {
        var balance = AccountState.GetBalance(currentState);
        var newBalance = balance + _amount;
        return currentState.WithProperty("Balance", newBalance);
    }

    public override OperationMetadata Metadata => base.Metadata with
    {
        Description = $"Deposits {_amount} to account",
        Parameters = new Dictionary<string, object> { ["Amount"] = _amount }
    };
}


