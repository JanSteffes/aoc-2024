using aoc_2024.AocClient;
using aoc_2024.Interfaces;
using Spectre.Console;

namespace aoc_2024.Classes
{
    public class ConsoleController : IController
    {
        private readonly IRunner runner;
        private readonly IAocClient aocClient;
        private readonly ILogger logger;
        private readonly ISolutionManager solutionManager;
        private readonly ILastExecutionManager lastExecutionManager;

        public ConsoleController(IRunner runner, IAocClient aocClient, ILogger logger,
            ISolutionManager solutionManager, ILastExecutionManager lastExecutionManager)
        {
            this.runner = runner;
            this.aocClient = aocClient;
            this.logger = logger;
            this.solutionManager = solutionManager;
            this.lastExecutionManager = lastExecutionManager;
        }

        public void Run()
        {
            bool shouldExit = false;

            while (!shouldExit)
            {
                Console.Clear();
                PrintHeader();
                Mode mode = SelectMode();

                switch (mode)
                {
                    case Mode.Run:
                        RunDay();
                        break;
                    case Mode.Test:
                        break;
                    case Mode.Init:
                        InitializeDay();
                        break;
                    case Mode.Repeat:
                        RunLastCommand();
                        break;
                    case Mode.Exit:
                        shouldExit = true;
                        break;
                    default:
                        break;
                }
            }
        }

        private static void PrintHeader()
        {
            Rule rule = new("🎄🎅❄️✨🎁🦌⛄🍪🌟🎄🎅❄️✨🎁🦌⛄🍪🌟🎄🎅❄️✨🎁🦌⛄🍪🌟🎄🎅❄️✨🎁🦌⛄🍪🌟")
            {
                Justification = Justify.Center,
                Border = BoxBorder.None,
            };

            AnsiConsole.Write(rule);

            AnsiConsole.Write(
                new FigletText("AoC 2024")
                .Centered()
                .Color(Color.Red));

            AnsiConsole.WriteLine();

            AnsiConsole.Write(rule);
        }

        private void RunLastCommand()
        {
            if (!this.lastExecutionManager.HasLastExecution())
            {
                this.logger.Log("Invalid last execution.", LogSeverity.Error);
                return;
            }

            ExecutionSettings execution = this.lastExecutionManager.GetLastExecution();

            if (execution.mode == Mode.Run)
            {
                this.logger.Log($"Running day {execution.day} part {execution.part}", LogSeverity.Log);
                // RunDay(dayNumber, part);
            }
            else if (execution.mode == Mode.Test)
            {
                if (execution.testNumber != 0)
                {
                    this.logger.Log($"Running test {execution.testNumber} for day {execution.day} part {execution.part}", LogSeverity.Log);
                    // TestDay(dayNumber, part, testNumber);
                }
                else
                {
                    this.logger.Log("Invalid test number or Test not found.", LogSeverity.Error);
                }
            }
            else
            {
                this.logger.Log("Invalid test mode.", LogSeverity.Error);
            }
        }

        private Mode SelectMode()
        {
            string[] baseChoices = [Mode.Run.ToString(), Mode.Test.ToString(), Mode.Init.ToString(), Mode.Exit.ToString()];

            string[] choices = this.lastExecutionManager.HasLastExecution() ?
                [GetLastExecutionString(), .. baseChoices] :
                [.. baseChoices];

            string modeText = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("[bold]Mode selection[/]")
                .AddChoices(choices)
                );

            if (!Enum.TryParse(modeText, out Mode mode))
            {
                return Mode.Repeat;
            }
            else
            {
                return mode;
            }
        }

        private static Part SelectPart()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<Part>()
                .UseConverter(m => m.ToString())
                .Title("[bold]Part selection[/]")
                .AddChoices(Enum.GetValues<Part>())
                );
        }

        private void RunDay()
        {
            int dayToRun = ChoseAvailableSolution();

            if (dayToRun == 0)
            {
                this.logger.Log("There isn't any available solution. Press any key to continue.", LogSeverity.Error);
                Console.ReadKey();
                return;
            }

            Part partToRun = SelectPart();

            Console.Clear();

            PrintHeader();

            this.lastExecutionManager.WriteLastChoice(dayToRun, Mode.Run, partToRun);

            AnsiConsole.WriteLine();

            AnsiConsole.Status()
                .Start("Running...", ctx =>
                {
                    ctx.Spinner(Spinner.Known.Star);
                    ctx.SpinnerStyle(Style.Parse("green"));
                    ISolution? solution = this.solutionManager.CreateSolutionInstance(dayToRun);
                    if (solution != null)
                    {
                        string input = this.solutionManager.ReadInputFile(dayToRun);
                        if (!string.IsNullOrEmpty(input))
                        {
                            this.runner.RunDay(solution, dayToRun, partToRun, input);
                        }
                    }
                    Thread.Sleep(5000);
                });

            this.logger.Log("Press any key to continue", LogSeverity.Other);
            Console.ReadKey();
        }

        private void InitializeDay()
        {
            int dayToInitialize = AnsiConsole.Prompt(
                new TextPrompt<int>("Day: "));

            if (this.solutionManager.IsDayAlreadyInitialized(dayToInitialize))
            {
                this.logger.Log($"Day #{dayToInitialize} already initialized. Press any key to continue.", LogSeverity.Error);
                Console.ReadKey();
                return;
            }

            Console.Clear();
            PrintHeader();

            AnsiConsole.Status()
                .Start($"Initializing day #{dayToInitialize}...", ctx =>
                {
                    ctx.Spinner(Spinner.Known.Star2);
                    AnsiConsole.MarkupLine("[blue]Getting puzzle input from AoC...[/]");
                    ClientResponse input = this.aocClient.GetPuzzleInput(dayToInitialize).Result;
                    if (input.ResponseType == ClientResponseType.Success)
                    {
                        AnsiConsole.MarkupLine("[green]Puzzle input fetched with success![/]");
                        ctx.Spinner(Spinner.Known.Christmas);
                        AnsiConsole.MarkupLine("[blue]Creating files...[/]");
                        this.solutionManager.CreateInitialFiles(dayToInitialize, input.Content);
                        AnsiConsole.MarkupLine($"[green]Files created for Day #{dayToInitialize}[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[red]Error getting puzzle input:[/] \r\n: {input.Content}");
                    }

                    AnsiConsole.MarkupLine("Press any key to return to main menu");
                });

            Console.ReadKey();
        }

        private int ChoseAvailableSolution()
        {
            int[] availableSolutions = this.solutionManager.GetAvailableSolutions();

            if (availableSolutions.Length == 0)
            {
                return 0;
            }

            int choice = AnsiConsole.Prompt(
                new SelectionPrompt<int>()
                .Title("[bold]Select day[/]")
                .PageSize(5)
                .AddChoices(availableSolutions)
                );

            return choice;
        }

        private string GetLastExecutionString()
        {
            if (!this.lastExecutionManager.HasLastExecution())
            {
                return string.Empty;
            }

            ExecutionSettings execution = this.lastExecutionManager.GetLastExecution();

            if (execution.mode == Mode.Test)
            {
                return $"Run Test #{execution.testNumber}" +
                    $" for Day #{execution.day} - Part {execution.part}";
            }

            return $"Run Day #{execution.day} - Part {execution.part}";
        }
    }
}
