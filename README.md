# Order Sensitivity Implementation

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg?style=for-the-badge&logo=apache&logoColor=white)
![Build Status](https://img.shields.io/badge/Build-Passing-success?style=for-the-badge)
![Tests](https://img.shields.io/badge/Tests-98%20Passing-success?style=for-the-badge)

A comprehensive C# .NET 10 implementation demonstrating **order sensitivity patterns** and **failure modes** in stateful systems. This project provides a practical framework for understanding how operation execution order affects system state and demonstrates common failure modes that arise from order sensitivity.

## 📋 Overview

This implementation provides a demonstration and validation framework for order sensitivity concepts in stateful systems. The implementation includes:

- **Core Models**: State, Operations, Execution Order, State Transitions
- **Patterns**: Order-sensitive and order-insensitive operation patterns
- **Systems**: Stateful, Event Sourcing, and Workflow systems
- **Examples**: UserAccount, Workflow, and Configuration scenarios
- **Failure Modes**: 5 common failure modes with demonstrations
- **Testing Strategies**: 4 comprehensive testing approaches

## 🏗️ Project Structure

```
OrderSensitivity/
├── src/
│   ├── OrderSensitivity.Core/          # Core models, patterns, systems, utilities
│   ├── OrderSensitivity.Examples/      # Concrete examples (UserAccount, Workflow, Configuration)
│   ├── OrderSensitivity.FailureModes/ # 5 failure mode demonstrations
│   ├── OrderSensitivity.Testing/       # Testing strategies (Sequence, PropertyBased, Replay, Differential)
│   └── OrderSensitivity.Demo/          # Console application demonstrating concepts
└── tests/
    ├── OrderSensitivity.Core.Tests/
    ├── OrderSensitivity.Examples.Tests/
    ├── OrderSensitivity.FailureModes.Tests/
    └── OrderSensitivity.Testing.Tests/
```

## Getting Started

### Prerequisites

- .NET 10.0 SDK or later
- Development environment (Visual Studio 2022, VS Code, or compatible IDE)

### Building

```bash
dotnet build OrderSensitivity.sln
```

### Running Tests

```bash
dotnet test
```

**Test Coverage**: 98 tests covering all core components, examples, failure modes, and testing strategies.

### Running Demo

```bash
cd src/OrderSensitivity.Demo
dotnet run
```

Execute specific demonstrations:

```bash
dotnet run -- order      # Order sensitivity patterns
dotnet run -- failure    # Failure modes demonstrations
dotnet run -- testing    # Testing strategies
dotnet run -- all        # Run all demos
```

## 📦 Key Components

### OrderSensitivity.Core
Core library containing:
- **Models**: `State`, `Operation`, `OperationSequence`, `ExecutionOrder`, `StateTransition`
- **Patterns**: `OrderSensitiveOperation`, `OrderInsensitiveOperation`, `StateDependentOperation`
- **Systems**: `StatefulSystem`, `EventSourcingSystem`, `WorkflowSystem`
- **Utilities**: `OrderValidator`, `StateComparer`

### OrderSensitivity.Examples
Concrete examples demonstrating order sensitivity:
- **UserAccount**: Deposit, Withdraw, ApplyFee operations
- **Workflow**: ValidateInput → ProcessPayment → SendNotification
- **Configuration**: SetDefault and Override operations

### OrderSensitivity.FailureModes
Demonstrations of 5 common failure modes:
1. **Replay Divergence**: Event replay producing different results
2. **Partial Re-Execution**: Retrying only part of a workflow
3. **Rollback Inconsistency**: Transaction rollback issues
4. **Workflow Drift**: Long-running workflow state changes
5. **Event Ordering Mistakes**: Distributed event processing errors

### OrderSensitivity.Testing
Testing strategies for order-sensitive systems:
1. **Sequence Testing**: Permutation-based testing
2. **Property-Based Testing**: Random sequence generation
3. **Replay Testing**: Record and replay validation
4. **Differential Testing**: Compare different execution orders

## Usage Examples

### Order Sensitivity Example

```csharp
// Demonstrates order sensitivity: execution order affects final state
var deposit = new DepositOperation(100m);
var applyFee = new ApplyFeeOperation(0.1m);

// Execution order 1: Deposit then ApplyFee results in final balance of 90m
var state1 = system.ExecuteSequence(new[] { deposit, applyFee });

// Execution order 2: ApplyFee then Deposit results in final balance of 100m
var state2 = system.ExecuteSequence(new[] { applyFee, deposit });
```

### Workflow Example

```csharp
var workflow = new WorkflowSystem();
workflow.AddStep(new ValidateInputOperation());
workflow.AddStep(new ProcessPaymentOperation(), dependsOn: "ValidateInput");
workflow.AddStep(new SendNotificationOperation(), dependsOn: "ProcessPayment");

// Execution respects dependency constraints
workflow.ExecuteAll();
```

## 🧪 Testing

The project includes comprehensive test coverage:

- **Core Tests**: 75 tests covering all core components
- **Examples Tests**: 9 tests for all example scenarios
- **Failure Modes Tests**: 7 tests for all failure modes
- **Testing Strategies Tests**: 7 tests for all testing approaches

**Total**: 98 tests with ~85% coverage of critical components.

## 📝 License

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

## ⚠️ Important Note

This implementation is intended for demonstration and validation purposes regarding order sensitivity concepts. It is not designed for production use. The implementation is structured for educational and research applications to analyze how operation execution order affects system behavior.

---

**Implementation**: C# .NET 10
