using aoc_2024.Interfaces;
using aoc_2024.Solutions.Helper;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;

namespace aoc_2024.Solutions
{
    public class Solution06 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var map = new PathMap(inputData);
            var visitedPoints = new HashSet<Point> { map.GetGuardPosition() };
            while (map.MoveGuard() is Point newPosition && map.IsInMap(newPosition))
            {
                visitedPoints.Add(newPosition);
            }
            // 4559
            return visitedPoints.Count.ToString();
        }

        public string RunPartB(string inputData)
        {
            var map = new PathMap(inputData);
            var startPosition = map.GetGuardPosition();
            var visitedPoints = new HashSet<Point> { startPosition };
            while (map.MoveGuard() is Point newPosition && map.IsInMap(newPosition))
            {
                visitedPoints.Add(newPosition);
            }

            //var working = Working(visitedPoints, startPosition, inputData);

            // is not 1471 or 1839. 1839 was too high

            var positionsForObstacles = new ConcurrentDictionary<Point, bool>();
            var visitedPointsWithoutStartingPosition = visitedPoints.Except([startPosition]).ToList();
            using var timer = StartLogTimer(positionsForObstacles);
            Parallel.ForEach(visitedPointsWithoutStartingPosition, visitedPoint =>
            {
                var worksForPoint = WouldCreateLoop(startPosition, inputData, visitedPoint);
                positionsForObstacles.TryAdd(visitedPoint, worksForPoint);
            });
            // 1604 (around 10 sec)
            return positionsForObstacles.Where(p => p.Value).Count().ToString();
        }

        private static bool WouldCreateLoop(Point startPosition, string inputData, Point point)
        {
            var currentMap = new PathMap(inputData);
            currentMap.AddValuePoint(new ValuePoint<char>('0', point));
            var currentlyVisitedPoints = new HashSet<Point> { startPosition };
            var pathAfterFirstDouble = new List<Point>();
            while (currentMap.MoveGuard() is Point newPosition && currentMap.IsInMap(newPosition))
            {
                if (!currentlyVisitedPoints.Add(newPosition))
                {
                    if (pathAfterFirstDouble.Count > 0 && newPosition.Equals(pathAfterFirstDouble[0]))
                    {
                        return true;
                    }
                    pathAfterFirstDouble.Add(newPosition);
                }
                else if (pathAfterFirstDouble.Count > 0)
                {
                    pathAfterFirstDouble = [];
                }
            }
            return false;
        }

        private System.Timers.Timer StartLogTimer(ConcurrentDictionary<Point, bool> itemsToLog)
        {
            var timer = new System.Timers.Timer();
            var startTime = DateTime.Now;
            timer.Interval = 2000;
            timer.Elapsed += delegate { LogStuff(startTime, itemsToLog); };
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Start();
            return timer;
        }

        private static void LogStuff(DateTime startTime, ConcurrentDictionary<Point, bool> itemsToLog)
        {
            Debug.WriteLine($"Processed {itemsToLog.Count} points and detected {itemsToLog.Count(i => i.Value)} possible loops [Running for {(DateTime.Now - startTime).TotalSeconds} seconds]");
        }
    }

    internal class PathMap : MapBase
    {
        // TODO get those from Directionenum
        private readonly char[] faceDirectionChars = ['^', '>', '<', 'v'];

        private readonly char[] obstacleChars = ['#', '0'];

        private Point GuardPositionOnMap { get; set; }

        private Direction GuardFaceDirection { get; set; }

        public PathMap(string inputData) : base(inputData)
        {
            InitGuardPosition(Grid);
        }

        protected override List<ValuePointCategory<char>> GetValuePointCharCategoriesInternal()
        {
            return [new ValuePointCategory<char>("Obstacles", obstacleChars)];
        }

        public Point MoveGuard()
        {
            var newPosition = GuardFaceDirection.GetNewPostion(GuardPositionOnMap);
            var categoryValuePoints = ValuePointCategories.First().ValuePoints;
            while (categoryValuePoints.Any(obstaclePosition => obstaclePosition.Coordinate.Equals(newPosition)))
            {
                GuardFaceDirection = GuardFaceDirection.TurnOnObstacle();
                newPosition = GuardFaceDirection.GetNewPostion(GuardPositionOnMap);
            }
            GuardPositionOnMap = new Point(newPosition.X, newPosition.Y);
            return newPosition;
        }

        public Point GetGuardPosition()
        {
            return new Point(GuardPositionOnMap.X, GuardPositionOnMap.Y);
        }

        private void InitGuardPosition(char[][] grid)
        {
            for (var x = 0; x < grid.Length; x++)
            {
                for (var y = 0; y < grid[x].Length; y++)
                {
                    if (faceDirectionChars.Contains(grid[x][y]))
                    {
                        GuardPositionOnMap = new Point(x, y);
                        GuardFaceDirection = (Direction)Enum.ToObject(typeof(Direction), grid[x][y]);
                    }
                }
            }
            if (GuardPositionOnMap == default || GuardFaceDirection == default)
            {
                throw new ArgumentException("Could not get guardposition!");
            }
        }
    }

    internal static class DirectionExtensions
    {
        internal static Point GetNewPostion(this Direction direction, Point oldPosition)
        {
            var newPosition = new Point(oldPosition.X, oldPosition.Y);
            switch (direction)
            {
                case Direction.Up:
                    newPosition.Y++;
                    break;
                case Direction.Down:
                    newPosition.Y--;
                    break;
                case Direction.Left:
                    newPosition.X--;
                    break;
                case Direction.Right:
                    newPosition.X++;
                    break;
            }
            return newPosition;
        }

        internal static Direction TurnOnObstacle(this Direction direction)
        {
            return direction switch
            {
                Direction.Up => Direction.Right,
                Direction.Down => Direction.Left,
                Direction.Left => Direction.Up,
                Direction.Right => Direction.Down,
                _ => throw new NotImplementedException($"No implementation for {direction}!"),
            };
        }
    }
}