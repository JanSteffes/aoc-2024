using aoc_2024.Interfaces;
using aoc_2024.Solutions.Helper;
using System.Drawing;

namespace aoc_2024.Solutions
{
    public class Solution10 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var map = new TopographicMap(inputData, 9, 1, 0);
            var maxValuesReachable = map.CalculateTrailheadScoreSum();
            return maxValuesReachable.ToString();
        }

        public string RunPartB(string inputData)
        {
            var map = new TopographicMap(inputData, 9, 1, 0);
            var maxValuesReachable = map.CalculateTrailheadScoreSum(true);
            return maxValuesReachable.ToString();
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

        public int CalculateTrailheadScoreSum(bool ratingCalculation = false)
        {
            var startingPoints = GetStartingPoints();
            // start with topleft to topRight and then down
            startingPoints = [.. startingPoints.OrderByDescending(p => p.Y).ThenBy(p => p.X)];
            var trailCountSum = 0;
            var allTrails = new HashSet<string>();
            foreach (var startinPoint in startingPoints)
            {
                DiscoverTrails(startinPoint, allTrails);
                var trailScore = ratingCalculation ? allTrails.Count : allTrails.Select(a => a.Split("->").Last()).Distinct().Count();
                trailCountSum += trailScore;
                allTrails = [];
            }
            return trailCountSum;
        }

        private void DiscoverTrails(Point startingPoint, HashSet<string> allTrails)
        {
            //PrintMap([startingPoint]);
            DiscoverTrails(startingPoint, [], allTrails);
        }

        private void DiscoverTrails(Point currentPoint, List<Point> currentTrailPoints, HashSet<string> allTrails)
        {
            currentTrailPoints.Add(currentPoint);
            //PrintMap(currentTrailPoints);
            var pointValue = GetValueAtPos(currentPoint);
            if (pointValue == MaxValueToReach)
            {
                var fullTrailPath = new Trail(currentTrailPoints).TrailPath();
                if (allTrails.Add(fullTrailPath))
                {
                    return;
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Trail already discovered: {fullTrailPath}");
                return;
            }
            var pointsAround = GetValidPointsAroundPoint(currentPoint).Where(p => GetValueAtPos(p) == pointValue + 1).ToList();
            foreach (var point in pointsAround)
            {
                DiscoverTrails(point, new List<Point>(currentTrailPoints), allTrails);
            }
        }


#pragma warning disable IDE0051 // Remove unused private members
        private void PrintMap(List<Point> points)
#pragma warning restore IDE0051 // Remove unused private members
        {
            //var sleepTime = 300;
            //Thread.Sleep(sleepTime);
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

    class Trail
    {
        public List<Point> TrailPoints { get; set; }
        public Trail(List<Point> currentTrailPoints)
        {
            TrailPoints = currentTrailPoints;
        }

        public string TrailPath()
        {
            return string.Join("->", TrailPoints.Select(p => $"{p.X}|{p.Y}"));
        }


    }
}