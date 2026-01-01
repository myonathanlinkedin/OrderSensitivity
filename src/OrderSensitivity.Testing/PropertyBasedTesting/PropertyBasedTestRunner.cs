using OrderSensitivity.Core.Models;

namespace OrderSensitivity.Testing.PropertyBasedTesting;

/// <summary>
/// Runs property-based tests.
/// </summary>
public class PropertyBasedTestRunner
{
    /// <summary>
    /// Tests a property across multiple random sequences.
    /// </summary>
    public PropertyTestResult TestProperty(
        Func<State, bool> property,
        IReadOnlyList<IOperation> availableOperations,
        State initialState,
        int numberOfSequences,
        Random? random = null)
    {
        random ??= new Random();
        var violations = new List<PropertyViolation>();

        for (int i = 0; i < numberOfSequences; i++)
        {
            var sequence = PropertyBasedTestGenerator.GenerateRandomSequence(
                availableOperations, random);
            var finalState = sequence.Execute(initialState);

            if (!property(finalState))
            {
                violations.Add(new PropertyViolation
                {
                    Sequence = sequence,
                    FinalState = finalState,
                    SequenceNumber = i
                });
            }
        }

        return new PropertyTestResult
        {
            Violations = violations,
            Passed = violations.Count == 0,
            TotalSequences = numberOfSequences
        };
    }
}

/// <summary>
/// Result of property-based testing.
/// </summary>
public record PropertyTestResult
{
    public List<PropertyViolation> Violations { get; init; } = new();
    public bool Passed { get; init; }
    public int TotalSequences { get; init; }
}

/// <summary>
/// Represents a property violation.
/// </summary>
public record PropertyViolation
{
    public OperationSequence Sequence { get; init; } = null!;
    public State FinalState { get; init; } = new();
    public int SequenceNumber { get; init; }
}


