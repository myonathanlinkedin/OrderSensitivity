using OrderSensitivity.Core.Models;
using OrderSensitivity.Core.Patterns;

namespace OrderSensitivity.Examples.Configuration;

/// <summary>
/// Sets default value - order-sensitive because it can overwrite overrides.
/// </summary>
public class SetDefaultOperation : OrderSensitiveOperation
{
    private readonly string _defaultValue;

    public SetDefaultOperation(string defaultValue)
    {
        _defaultValue = defaultValue;
    }

    public override string Name => $"SetDefault({_defaultValue})";

    public override State Execute(State currentState)
    {
        // Set default and apply to all keys that don't have values
        var newState = currentState.WithProperty("Default", _defaultValue);
        
        // Apply default to all existing keys that don't have explicit values
        var keysToUpdate = new Dictionary<string, object>();
        foreach (var (key, _) in currentState.Properties)
        {
            if (key != "Default" && !keysToUpdate.ContainsKey(key))
            {
                keysToUpdate[key] = _defaultValue;
            }
        }
        
        if (keysToUpdate.Count > 0)
        {
            newState = newState.WithProperties(keysToUpdate);
        }
        
        return newState;
    }

    public override OperationMetadata Metadata => base.Metadata with
    {
        Description = $"Sets default value to {_defaultValue}",
        Parameters = new Dictionary<string, object> { ["DefaultValue"] = _defaultValue }
    };
}


