using aoc_2024.Interfaces;
using aoc_2024.Solutions.Helper;

namespace aoc_2024.Solutions
{
    public class Solution16 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var isTest = false;
            var maze = new MazeMap(inputData);
            TestPrintColoredMap(isTest, maze);
            maze.EliminateDeadEnds();
            TestPrintColoredMap(isTest, maze);
            var pathScore = maze.CalculateLowestPathScore(isTest, 1000);
            return pathScore.ToString();
        }

        public string RunPartB(string inputData)
        {
            throw new NotImplementedException();
        }

        private void TestPrintColoredMap(bool isTest, MazeMap maze)
        {
            if (isTest)
            {
                maze.PrintColoredMap();
            }
        }
    }
}