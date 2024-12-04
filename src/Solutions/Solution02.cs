using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution02 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var reports = GetReportLevels(inputData);
            var validReports = reports.Count(ReportIsValid);
            return validReports.ToString();
        }

        public string RunPartB(string inputData)
        {
            var reports = GetReportLevels(inputData);
            var validReports = reports.Count(ReportOrReducedReportIsValid);
            return validReports.ToString();
        }

        public static List<int[]> GetReportLevels(string inputString)
        {
            return
                ParseUtils.ParseIntoLines(inputString)
                .Select(s =>
                    s.Split(" ")
                    .Select(int.Parse)
                    .ToArray()
                ).ToList();
        }

        private bool ReportIsValid(int[] report)
        {
            return ResolveValidationErros(report) == 0;
        }

        private bool ReportOrReducedReportIsValid(int[] report)
        {
            if (ResolveValidationErros(report) == 0)
            {
                return true;
            }

            var reducedReportSets = GetReducedReportSets(report);
            return reducedReportSets.Any(reducedReportSet => ResolveValidationErros(reducedReportSet) == 0);
        }
        private int[][] GetReducedReportSets(int[] report)
        {
            var reducedReportSets = new int[report.Length][];
            for (int indexToSkip = 0; indexToSkip < reducedReportSets.Length; indexToSkip++)
            {
                var reducedReportSet = new int[report.Length - 1];
                var indexToFill = 0;
                for (var index = 0; index < report.Length; index++)
                {
                    if (index == indexToSkip)
                    {
                        continue;
                    }
                    reducedReportSet[indexToFill++] = report[index];
                }
                reducedReportSets[indexToSkip] = reducedReportSet;
            }
            return reducedReportSets;
        }

        private static int ResolveValidationErros(int[] report)
        {
            var increasing = false;
            var validationErrors = 0;
            for (var index = 0; index < report.Length - 1; index++)
            {
                var currentNumber = report[index];
                var nextNumber = report[index + 1];
                if (index == 0)
                {
                    increasing = currentNumber < nextNumber;
                }
                if (NumbersAreEquall(currentNumber, nextNumber))
                {
                    validationErrors++;
                }
                if (!LevelDistanceIsOkay(currentNumber, nextNumber))
                {
                    validationErrors++;
                }
                if (!DirectionIsOkay(currentNumber, nextNumber, increasing))
                {
                    validationErrors++;
                }
                currentNumber = nextNumber;
                nextNumber = report[index + 1];
            }
            return validationErrors;
        }

        private static bool DirectionIsOkay(int currentNumber, int nextNumber, bool increasing)
        {
            if (increasing)
            {
                return nextNumber > currentNumber;
            }
            else
            {
                return nextNumber < currentNumber;
            }

        }

        private static bool NumbersAreEquall(int first, int second)
        {
            return first == second;
        }

        private static bool LevelDistanceIsOkay(int first, int second)
        {
            var dist = Math.Abs(first - second);
            return dist > 0 && dist <= 3;
        }
    }
}