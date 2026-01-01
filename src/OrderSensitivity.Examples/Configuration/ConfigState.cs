using OrderSensitivity.Core.Models;

namespace OrderSensitivity.Examples.Configuration;

/// <summary>
/// State for a configuration system.
/// </summary>
public static class ConfigState
{
    public static State Create()
    {
        return new State();
    }

    public static string? GetValue(State state, string key)
    {
        return state.GetProperty<string>(key);
    }

    public static string? GetDefault(State state)
    {
        return state.GetProperty<string>("Default");
    }
}


