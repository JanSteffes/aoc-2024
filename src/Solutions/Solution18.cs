using aoc_2024.Interfaces;
using aoc_2024.Solutions.Helper;
using aoc_2024.SolutionUtils;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;

namespace aoc_2024.Solutions
{
    public class Solution18 : ISolution
    {
        private const int MinBytesInput = 1024;
        private const int MinBytesTest = 12;
        private const int GridSizeInput = 71;
        private const int GridSizeTest = 7;
        private const int InputSizeTest = 150;

        public string RunPartA(string inputData)
        {
            var isTest = inputData.Length < InputSizeTest;
            var fallingBytesCount = isTest ? MinBytesTest : MinBytesInput;
            var parsedCoordinates = ParseUtils.ParseIntoLines(inputData).Select(q => q.Split(",")).Select(q => new Point(int.Parse(q.First()), int.Parse(q.Last()))).Take(fallingBytesCount).ToList();
            var mazeMapInput = GetMazeMapInput(isTest ? GridSizeTest : GridSizeInput, parsedCoordinates);
            var mazeMap = new MazeMap(mazeMapInput);
            var mazeFinder = new MazeFinder<char>(mazeMap);
            var path = mazeFinder.GetPathAStar(false);
            return (path.Count - 1).ToString();
        }

        public string RunPartB(string inputData)
        {
            var isTest = inputData.Length < InputSizeTest;
            var gridMaxes = isTest ? GridSizeTest : GridSizeInput;
            var fallingBytesCount = isTest ? MinBytesTest : MinBytesInput;

            var parsedCoordinates = ParseUtils.ParseIntoLines(inputData).Select(q => q.Split(",")).Select(q => new Point(int.Parse(q.First()), int.Parse(q.Last()))).ToList();

            var run = 1;
            var optionsArray = new int[parsedCoordinates.Count - fallingBytesCount - 1];
            for (var index = 0; index < optionsArray.Length; index++)
            {
                optionsArray[index] = fallingBytesCount + run;
                run++;
            }

            var resultIndexes = new ConcurrentBag<int>();
            var taken = 0;
            var chunked = optionsArray.Chunk(100).ToList();

            while (taken < chunked.Count)
            {
                var currentOptionsToProcess = chunked.Skip(taken).First();
                Debug.WriteLine($"Processing from {currentOptionsToProcess.First()} to {currentOptionsToProcess.Last()} (Chunk {taken} from {chunked.Count})");
                Parallel.ForEach(currentOptionsToProcess, currentFallingBytesCount =>
                {
                    var fallingBytes = parsedCoordinates.Take(currentFallingBytesCount).ToList();
                    var mazeMapInput = GetMazeMapInput(isTest ? GridSizeTest : GridSizeInput, fallingBytes);
                    var mazeMap = new MazeMap(mazeMapInput);
                    var mazeFinder = new MazeFinder<char>(mazeMap);
                    var path = mazeFinder.GetPathAStar(false);
                    if (path.Count == 0)
                    {
                        resultIndexes.Add(currentFallingBytesCount);
                    }
                });
                if (resultIndexes.Count > 0)
                {
                    var minIndex = resultIndexes.Min();
                    var minIndexByte = parsedCoordinates[minIndex - 1];
                    return $"{minIndexByte.X},{minIndexByte.Y}";
                }
                taken++;
            }
            return "0,0";
        }

        private static string GetMazeMapInput(int gridMaxes, List<Point> coordinates)
        {
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
    }
}