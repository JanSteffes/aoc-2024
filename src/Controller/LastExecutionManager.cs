using System.Text;

namespace aoc_2024.Controller
{
    public struct ExecutionSettings
    {
        public Mode mode;
        public int day;
        public Part part;
        public int testNumber;
    }

    public class LastExecutionManager
    {
        private ExecutionSettings lastExecutionSettings;
        private readonly ILogger logger;
        public bool HasValidLastExecution { get; private set; }

        public ExecutionSettings LastExecution { get { return lastExecutionSettings; } }

        public LastExecutionManager(ILogger logger)
        {
            this.HasValidLastExecution = false;
            this.logger = logger;
            GetLastExecution();
        }

        public void GetLastExecution()
        {
            string filePath = Path.Combine("ProgramUtils", "last-choice.txt");

            if (!File.Exists(filePath))
            {
                this.HasValidLastExecution = false;
                return;
            }

            Dictionary<string, string>? settings = ReadSettingsFromFile(filePath);

            if (settings == null)
            {
                this.logger.Log("Invalid file format or missing required settings for the last execution.", LogSeverity.Error);
                this.HasValidLastExecution = false;
                return;
            }

            if (!TryParseSettings(settings))
            {
                this.HasValidLastExecution = false;
            }

            this.HasValidLastExecution = true;
        }

        private bool TryParseSettings(Dictionary<string, string> settings)
        {
            if (!settings.TryGetValue("Day", out string? dayText) ||
                !settings.TryGetValue("Mode", out string? modeText) ||
                !settings.TryGetValue("Part", out string? partText))
            {
                this.logger.Log("Invalid file format or missing required settings.", LogSeverity.Error);
                return false;
            }

            if (!int.TryParse(dayText, out this.lastExecutionSettings.day))
            {
                this.logger.Log("Invalid day number.", LogSeverity.Error);
                return false;
            }

            if (!Enum.TryParse(modeText, out this.lastExecutionSettings.mode) ||
                !Enum.TryParse(partText, out this.lastExecutionSettings.part))
            {
                this.logger.Log("Invalid Mode or Part value.", LogSeverity.Error);
                return false;
            }

            if (this.lastExecutionSettings.mode == Mode.Test &&
                !settings.TryGetValue("TestNumber", out string? testNumberText)
                && !int.TryParse(testNumberText, out this.lastExecutionSettings.testNumber))
            {
                this.logger.Log("Invalid test number.", LogSeverity.Error);
                return false;
            }

            return true;
        }

        public void WriteLastChoice(int dayNumber, Mode mode, Part part, int? testNumber = null)
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
                this.logger.Log($"Last choice saved: Day {dayNumber}, Mode {mode}, Part {part}" +
                    (testNumber.HasValue ? $", Test {testNumber.Value}" : ""),
                    LogSeverity.Log);
                GetLastExecution();
                this.HasValidLastExecution = true;
            }
            catch (Exception ex)
            {
                this.logger.Log($"Failed to write last choice: {ex.Message}",
                    LogSeverity.Log);
                this.HasValidLastExecution = false;
            }
        }

        private Dictionary<string, string>? ReadSettingsFromFile(string filePath)
        {
            try
            {
                string[] fileContent = File.ReadAllLines(filePath);

                if (fileContent.Length == 0)
                {
                    this.logger.Log("Empty file.", LogSeverity.Error);
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
    }
}
