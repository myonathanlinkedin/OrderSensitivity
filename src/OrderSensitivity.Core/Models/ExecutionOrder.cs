namespace OrderSensitivity.Core.Models;

/// <summary>
/// Represents execution order for operations.
/// Tracks sequence numbers and validates ordering constraints.
/// </summary>
public class ExecutionOrder
{
    public IReadOnlyList<int> SequenceNumbers { get; }
    public Dictionary<string, int> OperationIndices { get; }

    public ExecutionOrder(IReadOnlyList<int> sequenceNumbers)
    {
        SequenceNumbers = sequenceNumbers;
        OperationIndices = new Dictionary<string, int>();
        for (int i = 0; i < sequenceNumbers.Count; i++)
        {
            OperationIndices[$"Operation_{i}"] = sequenceNumbers[i];
        }
    }

    public ExecutionOrder(IEnumerable<IOperation> operations)
    {
        var sequenceNumbers = new List<int>();
        OperationIndices = new Dictionary<string, int>();
        
        int index = 0;
        foreach (var operation in operations)
        {
            sequenceNumbers.Add(index);
            OperationIndices[operation.Name] = index;
            index++;
        }
        
        SequenceNumbers = sequenceNumbers;
    }

    /// <summary>
    /// Checks if the execution order is valid.
    /// </summary>
    public bool IsValid()
    {
        // Basic validation: sequence numbers should be non-negative and unique
        return SequenceNumbers.All(n => n >= 0) &&
               SequenceNumbers.Distinct().Count() == SequenceNumbers.Count;
    }

    /// <summary>
    /// Checks if an operation at a given position violates ordering constraints.
    /// </summary>
    public bool ViolatesConstraints(IOperation operation, int position)
    {
        // Basic check: position should be within bounds
        if (position < 0 || position >= SequenceNumbers.Count)
        {
            return true;
        }

        // Additional constraint checking can be added here
        return false;
    }

    /// <summary>
    /// Gets the position of an operation in the sequence.
    /// </summary>
    public int? GetPosition(string operationName)
    {
        return OperationIndices.TryGetValue(operationName, out var position) ? position : null;
    }
}


