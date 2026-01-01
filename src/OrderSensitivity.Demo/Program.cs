using OrderSensitivity.Demo.Demos;

namespace OrderSensitivity.Demo;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("========================================");
        Console.WriteLine("Order Sensitivity Demonstration");
        Console.WriteLine("========================================");
        Console.WriteLine();

        if (args.Length == 0)
        {
            ShowMenu();
            return;
        }

        var demo = args[0].ToLower();
        switch (demo)
        {
            case "order":
            case "1":
                OrderSensitivityDemo.Run();
                break;
            case "failure":
            case "2":
                FailureModesDemo.Run();
                break;
            case "testing":
            case "3":
                TestingStrategiesDemo.Run();
                break;
            case "all":
                OrderSensitivityDemo.Run();
                Console.WriteLine();
                FailureModesDemo.Run();
                Console.WriteLine();
                TestingStrategiesDemo.Run();
                break;
            default:
                Console.WriteLine($"Unknown demo: {demo}");
                ShowMenu();
                break;
        }
    }

    static void ShowMenu()
    {
        Console.WriteLine("Available demos:");
        Console.WriteLine("  1. order    - Order sensitivity patterns");
        Console.WriteLine("  2. failure  - Failure modes demonstrations");
        Console.WriteLine("  3. testing  - Testing strategies");
        Console.WriteLine("  all         - Run all demos");
        Console.WriteLine();
        Console.WriteLine("Usage: OrderSensitivity.Demo [order|failure|testing|all]");
    }
}


