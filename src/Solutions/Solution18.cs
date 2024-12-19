using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;
using System.Drawing;

namespace aoc_2024.Solutions
{
    public class Solution18 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var isTest = inputData.Length < 150;
            var mazeMapInput = GetMazeMapInput(isTest ? 7 : 70, isTest ? 12 : 1024, inputData);
            var mazeMap = new MazeMap(mazeMapInput);
            if (isTest)
            {
                mazeMap.PrintColoredMapToFileInBackground("C:\\Users\\JanSt\\OneDrive\\Dokumente\\AoC2024\\18\\maze.png");
            }
            var result = mazeMap.CalculateLowestPathScore(isTest, 0);
            return result.ToString();
        }

        private string GetMazeMapInput(int gridMaxes, int obstacles, string inputData)
        {
            var coordinates = ParseUtils.ParseIntoLines(inputData).Select(q => q.Split(",")).Select(q => new Point(int.Parse(q.First()), int.Parse(q.Last()))).Take(obstacles).ToList();
            var resultString = "";
            for (var row = 0; row < gridMaxes; row++)
            {
                for (var column = 0; column < gridMaxes; column++)
                {
                    if (row == 0 && column == 0)
                    {
                        resultString += "S";
                    }
                    else if (coordinates.Contains(new Point(column, row)))
                    {
                        resultString += "#";
                    }
                    else if (row == gridMaxes - 1 && column == gridMaxes - 1)
                    {
                        resultString += "E";
                    }
                    else
                    {
                        resultString += ".";
                    }
                }
                resultString += Environment.NewLine;
            }
            return resultString;
        }

        public string RunPartB(string inputData)
        {
            throw new NotImplementedException();
        }




    }
}