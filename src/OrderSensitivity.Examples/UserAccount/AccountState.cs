using OrderSensitivity.Core.Models;

namespace OrderSensitivity.Examples.UserAccount;

/// <summary>
/// State for a user account system.
/// </summary>
public static class AccountState
{
    public static State Create(decimal initialBalance = 0m)
    {
        return new State().WithProperty("Balance", initialBalance);
    }

    public static decimal GetBalance(State state)
    {
        return state.GetProperty<decimal>("Balance", 0m);
    }
}


