using OrderSensitivity.Core.Models;

namespace OrderSensitivity.Core.Utilities;

/// <summary>
/// Utilities for comparing states and transitions.
/// </summary>
public static class StateComparer
{
    /// <summary>
    /// Compares two states for equality.
    /// </summary>
    public static bool AreEqual(State? state1, State? state2)
    {
        if (ReferenceEquals(state1, state2))
        {
            return true;
        }

        if (state1 == null || state2 == null)
        {
            return false;
        }

        if (state1.Properties.Count != state2.Properties.Count)
        {
            return false;
        }

        foreach (var (key, value1) in state1.Properties)
        {
            if (!state2.Properties.TryGetValue(key, out var value2))
            {
                return false;
            }

            if (!Equals(value1, value2))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Gets the differences between two states.
    /// </summary>
    public static StateDifference GetDifference(State? state1, State? state2)
    {
        var differentProperties = new List<string>();
        var propertyDifferences = new Dictionary<string, (object? Original, object? Other)>();

        if (state1 == null && state2 == null)
        {
            return new StateDifference
            {
                DifferentProperties = differentProperties,
                PropertyDifferences = propertyDifferences
            };
        }

        if (state1 == null || state2 == null)
        {
            differentProperties.Add("State");
            propertyDifferences["State"] = (state1, state2);
            return new StateDifference
            {
                DifferentProperties = differentProperties,
                PropertyDifferences = propertyDifferences
            };
        }

        var allKeys = state1.Properties.Keys.Union(state2.Properties.Keys).ToList();

        foreach (var key in allKeys)
        {
            var hasValue1 = state1.Properties.TryGetValue(key, out var value1);
            var hasValue2 = state2.Properties.TryGetValue(key, out var value2);

            if (!hasValue1 || !hasValue2 || !Equals(value1, value2))
            {
                differentProperties.Add(key);
                propertyDifferences[key] = (value1, value2);
            }
        }

        return new StateDifference
        {
            DifferentProperties = differentProperties,
            PropertyDifferences = propertyDifferences
        };
    }

    /// <summary>
    /// Checks if two transitions produce the same final state.
    /// </summary>
    public static bool ProduceSameFinalState(StateTransition transition1, StateTransition transition2)
    {
        return AreEqual(transition1.FinalState, transition2.FinalState);
    }
}


