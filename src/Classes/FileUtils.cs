namespace aoc_2024.Classes
{
    public static class FileUtils
    {
        public static string? FindProjectFolder()
        {
            DirectoryInfo? currentDir = new(AppContext.BaseDirectory);

            while (currentDir != null)
            {
                if (Directory.GetFiles(currentDir.FullName, "*.csproj").Length > 0)
                {
                    return currentDir.FullName;
                }
                currentDir = currentDir.Parent;
            }

            return null;
        }
    }
}
