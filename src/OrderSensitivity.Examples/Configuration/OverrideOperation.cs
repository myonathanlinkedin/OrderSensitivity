using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Patterns;

namespace OrderSensitivity.Examples.Configuration;

/// <summary>
/// Overrides a specific key - order-sensitive because default can overwrite it.
/// </summary>
public class OverrideOperation : OrderSensitiveOperation
{
    private readonly string _key;
    private readonly string _value;

    public OverrideOperation(string key, string value)
    {
        _key = key;
        _value = value;
    }

    public override string Name => $"Override({_key}, {_value})";

    public override State Execute(State currentState)
    {
        return currentState.WithProperty(_key, _value);
    }

    public override OperationMetadata Metadata => base.Metadata with
    {
        Description = $"Overrides key {_key} with value {_value}",
        Parameters = new Dictionary<string, object>
        {
            ["Key"] = _key,
            ["Value"] = _value
        }
    };
}


