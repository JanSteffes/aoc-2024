namespace aoc_2024.SolutionUtils
{
    public static class ParseUtils
    {
        private static readonly string[] newLineSeparators = ["\r\n", "\n", "\r"];

        public static string[] ParseIntoLines(string input)
        {
            return input.Split(newLineSeparators, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
