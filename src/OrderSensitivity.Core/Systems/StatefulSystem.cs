using System;
using OrderSensitivity.Core.Models;

namespace OrderSensitivity.Core.Systems;

/// <summary>
/// Core stateful system that executes operations on state.
/// </summary>
public class StatefulSystem
{
    private State _currentState;

    public State CurrentState => _currentState;

    public StatefulSystem(State initialState)
    {
        _currentState = initialState;
    }

    public StatefulSystem() : this(new State())
    {
    }

    /// <summary>
    /// Executes a single operation on the current state.
    /// </summary>
    public State Execute(IOperation operation)
    {
        if (operation == null)
        {
            throw new ArgumentNullException(nameof(operation));
        }

        _currentState = operation.Execute(_currentState);
        return _currentState;
    }

    /// <summary>
    /// Executes a sequence of operations on the current state.
    /// </summary>
    public State ExecuteSequence(OperationSequence sequence)
    {
        if (sequence == null)
        {
            throw new ArgumentNullException(nameof(sequence));
        }

        if (!sequence.IsValid())
        {
            throw new InvalidOperationException("Operation sequence is not valid");
        }

        _currentState = sequence.Execute(_currentState);
        return _currentState;
    }

    /// <summary>
    /// Resets the system to the initial state.
    /// </summary>
    public void Reset(State initialState)
    {
        if (initialState == null)
        {
            throw new ArgumentNullException(nameof(initialState));
        }

        _currentState = initialState;
    }
}


