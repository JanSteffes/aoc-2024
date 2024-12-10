using aoc_2024.Interfaces;
using aoc_2024.Solutions.Helper;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Drawing;

namespace aoc_2024.Solutions
{
    public class Solution10 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var map = new TopographicMap(inputData, 9, 1, 0);
            var maxValuesToReach = map.CalculateTrailheadScoreSum();
            //JsonConvert.SerializeObject(map);
            return maxValuesToReach.ToString();
        }

        public string RunPartB(string inputData)
        {
            throw new NotImplementedException();
        }
    }

    class TopographicMap : MapBase
    {
        public int StepValue { get; private set; }

        public int MaxValueToReach { get; private set; }

        public int StartPointValue { get; private set; }

        public TopographicMap(string inputData, int maxValueToReach, int stepValue, int startPointValue) : base(inputData)
        {
            MaxValueToReach = maxValueToReach.ToString()[0];
            StepValue = stepValue;
            StartPointValue = startPointValue.ToString()[0];
        }

        public int CalculateTrailheadScoreSum()
        {
            var gridSerialized = JsonConvert.SerializeObject(Grid);
            var startingPoints = GetStartingPoints();
            startingPoints = startingPoints.OrderByDescending(p => p.Y).ThenBy(p => p.X).ToList();
            var trailHeadScores = new ConcurrentBag<int>();
            //Parallel.ForEach(startingPoints, startingPoint =>
            //{
            foreach (var startingPoint in startingPoints)
            {
                var trailHeadScore = DiscoverTrailScore(startingPoint);
                trailHeadScores.Add(trailHeadScore);
                Console.WriteLine($"Found {trailHeadScore} valid paths. Press key for next run.");
                Console.ReadLine();
            }
            //});
            return trailHeadScores.Sum();
        }

        private void PrintMap(List<Point> points)
        {
            for (var y = Grid.Length - 1; y >= 0; y--)
            {
                for (var x = 0; x < Grid.Length; x++)
                {
                    var currentPoint = new Point(x, y);
                    Console.ForegroundColor = GetColorForPoint(currentPoint, points);
                    Console.Write(GetValueAtPos(currentPoint));
                }
                Console.WriteLine();
            }
            for (var i = 0; i < 6; i++)
            {
                Console.WriteLine(Environment.NewLine);
            }
        }

        private ConsoleColor GetColorForPoint(Point currentPoint, List<Point> points)
        {
            if (points.Any(p => p.Equals(currentPoint)))
            {
                if (points.First().Equals(currentPoint))
                {
                    return ConsoleColor.Red;
                }
                if (GetValueAtPos(currentPoint) == MaxValueToReach)
                {
                    return ConsoleColor.DarkGreen;
                }
                return ConsoleColor.Blue;
            }
            return ConsoleColor.White;
        }

        private int DiscoverTrailScore(Point startingPoint, List<Point>? points = null)
        {
            Thread.Sleep(500);
            points ??= [];
            if (!points.Contains(startingPoint))
            {
                points.Add(startingPoint);
                PrintMap(points);
            }
            var currentPointValue = GetValueAtPos(startingPoint);
            if (currentPointValue == MaxValueToReach)
            {
                Console.WriteLine("Found another valid path. Press any key to continue.");
                Console.Read();
                return 1;
            }
            var pointsAroundStartingPoint = GetValidPointsAroundPoint(startingPoint);
            var sum = 0;
            foreach (var point in pointsAroundStartingPoint)
            {
                if (GetValueAtPos(point) == GetValueAtPos(startingPoint) + 1)
                {
                    sum += DiscoverTrailScore(point, points);
                }
            }
            return sum;
        }

        private char GetValueAtPos(Point startingPoint)
        {
            return Grid[startingPoint.X][startingPoint.Y];
        }

        private List<Point> GetValidPointsAroundPoint(Point startingPoint)
        {
            var points = new List<Point>();
            for (int value = -1; value < 2; value += 2)
            {
                TryAddPoint(points, startingPoint, new Point(startingPoint.X + value, startingPoint.Y));
                TryAddPoint(points, startingPoint, new Point(startingPoint.X, startingPoint.Y + value));
            }
            return points;
        }

        private void TryAddPoint(List<Point> points, Point startingPoint, Point point)
        {
            if (!point.Equals(startingPoint) && IsInMap(point))
            {
                points.Add(point);
            }
        }

        private List<Point> GetStartingPoints()
        {
            var startingPoints = new List<Point>();
            for (int x = 0; x < Grid.Length; x++)
            {
                var currentRow = Grid[x];
                for (int y = 0; y < currentRow.Length; y++)
                {
                    var newPoint = new Point(x, y);
                    if (GetValueAtPos(newPoint) == StartPointValue)
                    {
                        startingPoints.Add(newPoint);
                    }
                }
            }
            return startingPoints;
        }


    }
}