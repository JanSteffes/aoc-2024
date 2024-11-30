using aoc_2024.AocClient;
using aoc_2024.Classes;
using aoc_2024.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace aoc_2024
{
    public class Program
    {
        static void Main()
        {
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddSingleton<ILogger, ConsoleLogger>()
                .AddSingleton<ISolutionManager, SolutionManager>()
                .AddSingleton<ILastExecutionManager, LastExecutionManager>()
                .AddSingleton<IAocClient, AocHttpClient>()
                .AddSingleton<IRunner, ConsoleRunner>()
                .AddSingleton<IController, ConsoleController>()
                .BuildServiceProvider();

            IController inputController = serviceProvider.GetRequiredService<IController>();
            inputController.Run();
        }
    }
}
