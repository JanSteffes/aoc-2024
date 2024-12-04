using aoc_2024.Interfaces;
using aoc_2024.Solutions.Helper;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution04 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var lines = ParseUtils.ParseIntoLines(inputData);
            var overallFindings = 0;
            var horizontalStrings = Solution4Helper.GetHorizontalLines(lines);
            overallFindings += Solution4Helper.SearchDirectionForString(horizontalStrings, "horizontal");

            var verticalStrings = Solution4Helper.GetVertialLines(lines);
            overallFindings += Solution4Helper.SearchDirectionForString(verticalStrings, "vertical");

            var diagonalStrings = Solution4Helper.GetDiagonalLines(lines);
            overallFindings += Solution4Helper.SearchDirectionForString(diagonalStrings, "diagonal");

            return overallFindings.ToString();
        }

        public string RunPartB(string inputData)
        {
            var coordinateSystem = BuildCoordinateSystemFromString(inputData);
            var possibleXmasCenters = GetPossibleXmasCenters(coordinateSystem);
            var xmases = GetXmasesCenters(coordinateSystem, possibleXmasCenters);
            return xmases.Count.ToString();

        }

        /// <summary>
        /// check each possible xmas-centers (being an 'A') if x-1,y+1 and x+1,y-1 as well as x-1,y-1 and x+1,y-1 produces sam or mas
        /// </summary>
        /// <param name="coordinateSystem"></param>
        /// <param name="possibleXmasCenters"></param>
        /// <returns></returns>
        private static List<(int X, int Y)> GetXmasesCenters(char[][] coordinateSystem, List<(int X, int Y)> possibleXmasCenters)
        {
            var xmasCenters = new List<(int X, int Y)>();
            foreach (var center in possibleXmasCenters)
            {
                if (IsMas(coordinateSystem[center.X - 1][center.Y - 1], coordinateSystem[center.X][center.Y], coordinateSystem[center.X + 1][center.Y + 1])
                    && IsMas(coordinateSystem[center.X - 1][center.Y + 1], coordinateSystem[center.X][center.Y], coordinateSystem[center.X + 1][center.Y - 1]))
                {
                    xmasCenters.Add(center);
                }
            }
            return xmasCenters;
        }

        private static bool IsMas(params char[] chars)
        {
            var stringToCheck = new string(chars);
            return stringToCheck == "MAS" || stringToCheck == "SAM";
        }

        private static char[][] BuildCoordinateSystemFromString(string inputData)
        {
            var lines = ParseUtils.ParseIntoLines(inputData);
            var length = lines.Length;
            var coordinateSystem = new char[length][];
            for (int column = 0; column < length; column++)
            {
                coordinateSystem[column] = new char[length];
                for (int row = 0; row < length; row++)
                {
                    coordinateSystem[column][row] = lines[length - 1 - row][column];
                }
            }
            return coordinateSystem;
        }

        /// <summary>
        /// check each coordinate where x and y are at least 1 smaller than bounds if it's an a.
        /// </summary>
        /// <param name="coordinateSystem"></param>
        /// <returns></returns>
        private static List<(int X, int Y)> GetPossibleXmasCenters(char[][] coordinateSystem)
        {
            var possibleXmasCenters = new List<(int X, int Y)>();
            var bounds = coordinateSystem.Length;
            for (var column = 1; column < bounds - 1; column++)
            {
                for (var row = 1; row < bounds - 1; row++)
                {
                    if (coordinateSystem[column][row] == 'A')
                    {
                        possibleXmasCenters.Add((column, row));
                    }
                }
            }
            return possibleXmasCenters;
        }

    }
}