using aoc_2024.Runner;
using Spectre.Console;
using System.Text;

namespace aoc_2024.Controller
{
    public class ConsoleController : IController
    {
        private struct ExecutionSettings
        {
            public Mode mode;
            public int day;
            public Part part;
            public int testNumber;
            public string executionString;
        }

        private ExecutionSettings lastExecutionSettings;
        private bool hasValidLastExecution;
        private readonly IRunner runner;

        public ConsoleController(IRunner runner)
        {
            this.runner = runner;
            this.hasValidLastExecution = GetLastExecution();
        }

        public void Run()
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
                default:
                    break;
            }
        }

        private static void PrintHeader()
        {
            Rule rule = new("🎄🎅❄️✨🎁🦌⛄🍪🌟🎄🎅❄️✨🎁🦌⛄🍪🌟🎄🎅❄️✨🎁🦌⛄🍪🌟🎄🎅❄️✨🎁\U0001f98c⛄🍪🌟")
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
            if (!this.hasValidLastExecution)
            {
                PrintError("Invalid last execution.");
                return;
            }

            if (this.lastExecutionSettings.mode == Mode.Run)
            {
                PrintMessage($"Running day {this.lastExecutionSettings.day} part {this.lastExecutionSettings.part}");
                // RunDay(dayNumber, part);
            }
            else if (this.lastExecutionSettings.mode == Mode.Test)
            {
                if (this.lastExecutionSettings.testNumber != 0)
                {
                    PrintMessage($"Running test {this.lastExecutionSettings.testNumber} for day {this.lastExecutionSettings.day} part {this.lastExecutionSettings.part}");
                    // TestDay(dayNumber, part, testNumber);
                }
                else
                {
                    PrintError("Invalid test number or Test not found.");
                }
            }
            else
            {
                PrintError("Invalid test mode.");
            }
        }

        private Mode SelectMode()
        {
            string[] choices;

            if (this.hasValidLastExecution)
            {
                choices = [this.lastExecutionSettings.executionString, Mode.Run.ToString(), Mode.Test.ToString(), Mode.Init.ToString()];
            }
            else
            {
                choices = [Mode.Run.ToString(), Mode.Test.ToString(), Mode.Init.ToString()];
            }

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
            int dayToRun = AnsiConsole.Prompt(
                new TextPrompt<int>("Day: "));

            Part partToRun = SelectPart();

            Console.Clear();

            PrintHeader();

            WriteLastChoice(dayToRun, Mode.Run, partToRun);

            AnsiConsole.WriteLine();

            AnsiConsole.Status()
                .Start("Running...", ctx =>
                {
                    ctx.Spinner(Spinner.Known.Star);
                    ctx.SpinnerStyle(Style.Parse("green"));
                    this.runner.RunDay(dayToRun, partToRun);
                    Thread.Sleep(5000);
                });

        }

        private void InitializeDay()
        {
            this.runner.InitializeDay(1);
        }

        private void PrintError(string errorMessage)
        {
            AnsiConsole.Write(new Markup($"[bold red]Error:[/] {errorMessage}"));
        }

        private void PrintMessage(string message)
        {
            AnsiConsole.Write(new Markup($"[bold green]Log:[/] {message}"));
        }

        private bool GetLastExecution()
        {
            string filePath = Path.Combine("ProgramUtils", "last-choice.txt");

            if (!File.Exists(filePath))
            {
                return false;
            }

            Dictionary<string, string>? settings = ReadSettingsFromFile(filePath);

            if (settings == null)
            {
                PrintError("Invalid file format or missing required settings for the last execution.");
                return false;
            }

            if (!TryParseSettings(settings))
            {
                return false;
            }

            return true;
        }

        private Dictionary<string, string>? ReadSettingsFromFile(string filePath)
        {
            try
            {
                string[] fileContent = File.ReadAllLines(filePath);

                if (fileContent.Length == 0)
                {
                    PrintError("Empty file.");
                    return null;
                }

                Dictionary<string, string> settings = [];

                foreach (string line in fileContent)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    {
                        continue;
                    }

                    string[] parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        settings[parts[0].Trim()] = parts[1].Trim();
                    }
                }

                return settings;
            }
            catch
            {
                return null;
            }
        }

        private bool TryParseSettings(Dictionary<string, string> settings)
        {
            if (!settings.TryGetValue("Day", out string? dayText) ||
                !settings.TryGetValue("Mode", out string? modeText) ||
                !settings.TryGetValue("Part", out string? partText))
            {
                PrintError("Invalid file format or missing required settings.");
                return false;
            }

            if (!int.TryParse(dayText, out this.lastExecutionSettings.day))
            {
                PrintError("Invalid day number.");
                return false;
            }

            if (!Enum.TryParse(modeText, out this.lastExecutionSettings.mode) ||
                !Enum.TryParse(partText, out this.lastExecutionSettings.part))
            {
                PrintError("Invalid Mode or Part value.");
                return false;
            }

            if (this.lastExecutionSettings.mode == Mode.Test &&
                !settings.TryGetValue("TestNumber", out string? testNumberText)
                && !int.TryParse(testNumberText, out this.lastExecutionSettings.testNumber))
            {
                PrintError("Invalid test number.");
                return false;
            }

            if (this.lastExecutionSettings.mode == Mode.Test)
            {
                this.lastExecutionSettings.executionString = $"Run Test #{this.lastExecutionSettings.testNumber}" +
                    $" for Day #{this.lastExecutionSettings.day} - Part {this.lastExecutionSettings.part}";
            }
            else
            {
                this.lastExecutionSettings.executionString = $"Run Day #{this.lastExecutionSettings.day} - Part {this.lastExecutionSettings.part}";
            }

            return true;
        }

        private void WriteLastChoice(int dayNumber, Mode mode, Part part, int? testNumber = null)
        {
            string folderPath = Path.Combine("ProgramUtils");
            string filePath = Path.Combine(folderPath, "last-choice.txt");

            Directory.CreateDirectory(folderPath);

            StringBuilder content = new();
            content.AppendLine($"Day={dayNumber}");
            content.AppendLine($"Mode={mode}");
            content.AppendLine($"Part={part}");

            if (testNumber.HasValue)
            {
                content.AppendLine($"TestNumber={testNumber.Value}");
            }

            try
            {
                File.WriteAllText(filePath, content.ToString());
                PrintMessage($"Last choice saved: Day {dayNumber}, Mode {mode}, Part {part}" +
                    (testNumber.HasValue ? $", Test {testNumber.Value}" : ""));
            }
            catch (Exception ex)
            {
                PrintError($"Failed to write last choice: {ex.Message}");
            }
        }
    }
}
