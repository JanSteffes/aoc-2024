namespace aoc_2024.Controller
{
    public class SolutionManager
    {
        public int[] AvailableSolutions { get; private set; }

        public SolutionManager()
        {
            this.AvailableSolutions = GetAvailableSolutions();
        }

        public void CreateInitialFiles(int dayToInitialize, string inputContent)
        {
            CreateSolutionFile(dayToInitialize);
            CreateInputFile(dayToInitialize, inputContent);
            this.AvailableSolutions = GetAvailableSolutions();
        }

        public bool IsDayAlreadyInitialized(int dayToInitialize)
        {
            return this.AvailableSolutions.Contains(dayToInitialize);
        }

        private static int[] GetAvailableSolutions()
        {
            string solutionsFolder = Path.Combine("Solutions");

            return new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, @"..\..\..\Solutions"))
                .GetFiles("*.cs")
                .Select(file => int.Parse(Path.GetFileNameWithoutExtension(file.Name).Replace("Solution", "")))
                .OrderByDescending(x => x)
                .ToArray();
        }

        private static void CreateInputFile(int dayNumber, string inputText)
        {
            string? basePath = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;

            if (string.IsNullOrEmpty(basePath)) return;

            string folderPath = Path.Combine(basePath, "Inputs");
            string filePath = Path.Combine(folderPath, $"input-{dayNumber.ToString().PadLeft(2, '0')}.txt");

            Directory.CreateDirectory(folderPath);

            File.WriteAllText(filePath, inputText);
        }

        private static void CreateSolutionFile(int dayToInitialize)
        {
            string? basePath = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;

            if (string.IsNullOrEmpty(basePath)) return;

            string formatedDayNumber = $"Solution{dayToInitialize.ToString().PadLeft(2, '0')}";
            string folderPath = Path.Combine(basePath, "Solutions");
            string filePath = Path.Combine(folderPath, formatedDayNumber + ".cs");

            Directory.CreateDirectory(folderPath);

            string templatePath = Path.Combine("Templates", "solution-template.txt");
            string templateContent = File.ReadAllText(templatePath);

            string formatedTemplate = string.Format(templateContent, formatedDayNumber);

            File.WriteAllText(filePath, formatedTemplate);
        }
    }
}
