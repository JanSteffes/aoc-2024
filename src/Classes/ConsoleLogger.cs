using aoc_2024.Interfaces;
using Spectre.Console;

namespace aoc_2024.Classes
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message, LogSeverity logSeverity)
        {
            switch (logSeverity)
            {
                case LogSeverity.Error:
                    AnsiConsole.MarkupLine($"[bold red]Error:[/] {message}");
                    break;
                case LogSeverity.Log:
                    AnsiConsole.MarkupLine($"[bold green]Log:[/] {message}");
                    break;
                case LogSeverity.Runner:
                    AnsiConsole.MarkupLine($"[bold blue]Runner:[/] {message}");
                    break;
                default:
                    AnsiConsole.MarkupLine(message);
                    break;
            }
        }
    }
}
