using System;

namespace OrderSensitivity.Core.Models;

/// <summary>
/// Represents system state at a point in time.
/// States are immutable and comparable.
/// </summary>
public record State
{
    public Dictionary<string, object> Properties { get; init; } = new();
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public State()
    {
    }

    public State(Dictionary<string, object> properties)
    {
        Properties = new Dictionary<string, object>(properties);
    }

    /// <summary>
    /// Creates a new state with an updated property.
    /// </summary>
    /// <param name="key">The property key. Must not be null.</param>
    /// <param name="value">The property value. May be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when key is null.</exception>
    public State WithProperty(string key, object value)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        var newProperties = new Dictionary<string, object>(Properties)
        {
            [key] = value
        };
        return this with { Properties = newProperties };
    }

    /// <summary>
    /// Creates a new state with multiple updated properties.
    /// </summary>
    public State WithProperties(Dictionary<string, object> updates)
    {
        var newProperties = new Dictionary<string, object>(Properties);
        foreach (var (key, value) in updates)
        {
            newProperties[key] = value;
        }
        return this with { Properties = newProperties };
    }

    /// <summary>
    /// Gets a property value, returning default if not found.
    /// </summary>
    public T? GetProperty<T>(string key, T? defaultValue = default)
    {
        if (Properties.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return defaultValue;
    }

    /// <summary>
    /// Checks if state has a property.
    /// </summary>
    public bool HasProperty(string key)
    {
        return Properties.ContainsKey(key);
    }
}


