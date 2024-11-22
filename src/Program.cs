using aoc_2024.AocClient;
using aoc_2024.Controller;
using aoc_2024.MessageWriter;
using aoc_2024.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace aoc_2024
{
    internal class Program
    {
        static void Main()
        {
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddSingleton<IAocClient, AocHttpClient>()
                .AddSingleton<IMessageWriter, ConsoleMessageWriter>()
                .AddSingleton<IRunner, ConsoleRunner>()
                .AddSingleton<IController, ConsoleController>()
                .BuildServiceProvider();

            IController inputController = serviceProvider.GetRequiredService<IController>();
            inputController.Run();
        }
    }
}
