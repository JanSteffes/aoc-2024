namespace aoc_2024.Solutions.Helper
{
    internal static class Solution4Helper
    {
        public static int SearchDirectionForString(IEnumerable<string> lines, string ident)
        {
            //Debug.WriteLine(ident + ":");
            var findingsNormal = SearchLinesForString(lines);
            //Debug.WriteLine("");
            //Debug.WriteLine(ident + "- reversed:");
            var findingsReversed = SearchLinesForString(lines.Select(h => new string(h.Reverse().ToArray())));
            //Debug.WriteLine("");
            //Debug.WriteLine("######################");
            //Debug.WriteLine("");
            return findingsNormal + findingsReversed;
        }


        private static int SearchLinesForString(IEnumerable<string> lines)
        {
            var allFindings = 0;
            var lineNumber = 0;
            foreach (var line in lines)
            {
                var findings = SearchLineForString(line);
                if (findings > 0)
                {
                    //Debug.WriteLine($"Found {findings} in line {lineNumber + 1}/{lines.Count()}: \"{line}\"");
                    allFindings += findings;
                }
                lineNumber++;
            }
            return allFindings;
        }

        private static int SearchLineForString(string line)
        {
            var findings = 0;
            var charsToMatch = "XMAS";
            var charToMatchIndex = 0;
            var currentCharToMatch = charsToMatch[charToMatchIndex];
            foreach (var currentChar in line)
            {
                if (currentChar == currentCharToMatch)
                {
                    charToMatchIndex++;
                    if (charToMatchIndex == charsToMatch.Length)
                    {
                        findings++;
                        charToMatchIndex = 0;
                    }
                    currentCharToMatch = charsToMatch[charToMatchIndex];
                }
                else
                {
                    charToMatchIndex = currentChar == charsToMatch[0] ? 1 : 0;
                    currentCharToMatch = charsToMatch[charToMatchIndex];
                }
            }
            return findings;

        }

        internal static IEnumerable<string> GetVertialLines(string[] lines)
        {
            var verticalLines = new List<string>();
            for (var column = 0; column < lines.Length; column++)
            {
                var currentVerticalLine = "";
                foreach (var line in lines)
                {
                    currentVerticalLine += line[column];
                }
                verticalLines.Add(currentVerticalLine);
            }
            return verticalLines;
        }

        internal static List<string> GetHorizontalLines(string[] lines)
        {
            var horizonalLines = lines.ToList();
            return horizonalLines;
        }

        internal static IEnumerable<string> GetDiagonalLines(string[] lines)
        {
            var diagonalLines = new List<string>();
            for (var row = 0; row < lines.Length; row++)
            {
                var diagonalLinesOfRow = BuildDiagonalLines(lines, row);
                diagonalLines.AddRange(diagonalLinesOfRow);
            }
            return diagonalLines;
        }

        private static IEnumerable<string> BuildDiagonalLines(string[] lines, int startingRow)
        {
            var diagonalLines = new List<string>();
            for (var column = 0; column < lines.Length; column++)
            {
                if (startingRow == 0 || column == 0)
                {
                    var diagonalRightLine = BuildRightLine(lines, startingRow, column);
                    diagonalLines.Add(diagonalRightLine);
                }
                if (startingRow == 0 || column == lines.Length - 1)
                {
                    var diagonalLeftLine = BuildLeftLine(lines, startingRow, column);
                    diagonalLines.Add(diagonalLeftLine);
                }
            }
            return diagonalLines;
        }

        private static string BuildLeftLine(string[] lines, int startingRow, int startingColumn)
        {
            var diagonalLeftLineChars = new List<char>();
            var column = startingColumn;
            var currentRow = startingRow;
            while (lines.Length > currentRow && column >= 0)
            {
                diagonalLeftLineChars.Add(lines[currentRow++][column--]);
            }
            var line = new string(diagonalLeftLineChars.ToArray());
            return line;
        }

        private static string BuildRightLine(string[] lines, int startingRow, int startingColumn)
        {
            var diagonalRightLineChars = new List<char>();
            var column = startingColumn;
            var currentRow = startingRow;
            while (lines.Length > currentRow && lines[currentRow].Length > column)
            {
                diagonalRightLineChars.Add(lines[currentRow++][column++]);
            }
            var line = new string(diagonalRightLineChars.ToArray());
            return line;
        }

    }
}
