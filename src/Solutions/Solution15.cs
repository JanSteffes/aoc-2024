using aoc_2024.Interfaces;
using aoc_2024.Solutions.Helper;
using aoc_2024.SolutionUtils;
using Spectre.Console;
using System.Drawing;

namespace aoc_2024.Solutions
{
    public class Solution15 : ISolution
    {
        public string RunPartA(string inputData)
        {
            ParseInputToMap(inputData, out List<string> mapLines, out List<string> moveLines);

            var map = new WarehouseMap(string.Join(Environment.NewLine, mapLines));

            //PrintColoredMap(map);

            var moveSets = ParseDirections(string.Join(string.Empty, moveLines));
            foreach (var move in moveSets)
            {
                map.TryMoveWorker(move);
                //PrintColoredMap(map);
            }

            var checkSum = map.CalculateCheckSum();

            return checkSum.ToString();
        }

        private static void ParseInputToMap(string inputData, out List<string> mapLines, out List<string> moveLines)
        {
            var lines = ParseUtils.ParseIntoLines(inputData);
            mapLines = [];
            moveLines = [];
            foreach (var line in lines)
            {
                if (line.Contains('#'))
                {
                    mapLines.Add(line);
                }
                else
                {
                    moveLines.Add(line);
                }
            }
        }

        private void PrintColoredMap(MapBase map)
        {
            Thread.Sleep(500);
            var colorsForCategories = GetColorsForPointsInCategories(map.GetValuePointCategories());
            map.PrintMap(colorsForCategories);
        }

        private Dictionary<Point, ConsoleColor> GetColorsForPointsInCategories(List<ValuePointCategory<char>> list)
        {
            var resultDict = new Dictionary<Point, ConsoleColor>();
            ConsoleColor[] quadrantColors = [ConsoleColor.Green, ConsoleColor.Red, ConsoleColor.Blue, ConsoleColor.Magenta, ConsoleColor.Yellow, ConsoleColor.Cyan, ConsoleColor.Gray];
            var colorIndex = 0;
            foreach (var category in list)
            {
                if (quadrantColors.Length == colorIndex)
                {
                    colorIndex = 0;
                }
                var color = quadrantColors[colorIndex++];
                foreach (var point in category.ValuePoints)
                {
                    resultDict.Add(point.Coordinate, color);
                }
            }
            return resultDict;
        }

        public string RunPartB(string inputData)
        {
            throw new NotImplementedException();
        }


        private List<Direction> ParseDirections(string moveset)
        {
            return moveset.Select(c => (Direction)Enum.ToObject(typeof(Direction), c)).ToList();
        }


    }


    class WarehouseMap : MapBase
    {
        private const string UnmoveableCategoryName = "Unmoveable";
        private const string MoveableCategoryName = "Moveable";
        private const string WorkerCategoryName = "Worker";
        private readonly char[] unmoveableObstacles = ['#'];

        private readonly char[] moveableObstacles = ['O'];

        private readonly char[] workerChar = ['@'];

        public WarehouseMap(string inputData) : base(inputData)
        {
        }

        internal void TryMoveWorker(Direction move)
        {
            var workerValuePoints = GetValuePointCategoryByName(WorkerCategoryName);
            var currentPos = workerValuePoints.ValuePoints.First();
            var nextPos = GetNextPos(currentPos.Coordinate, move);
            var unmoveableObject = GetValuePointCategoryByName(UnmoveableCategoryName);
            // if newPos is unmoveableObstalce do nothing
            if (unmoveableObject.ContainsPoint(nextPos))
            {
                return;
            }
            // if newPos is moveable, check if it moves in the direction (recursive?)
            var moveableObjects = GetValuePointCategoryByName(MoveableCategoryName);
            if (moveableObjects.ContainsPoint(nextPos))
            {
                if (MoveMoveablePoint(unmoveableObject, moveableObjects, nextPos, move))
                {
                    MoveWorkerToNewPos(workerValuePoints, nextPos);
                }
                return;
            }
            // if newPos is free, move
            MoveWorkerToNewPos(workerValuePoints, nextPos);

        }

        private bool MoveMoveablePoint(ValuePointCategory<char> unmoveableObjects, ValuePointCategory<char> moveableObjects, Point moveableObjectPos, Direction direction)
        {
            var nextPos = GetNextPos(moveableObjectPos, direction);
            if (unmoveableObjects.ContainsPoint(nextPos))
            {
                return false;
            }
            if (moveableObjects.ContainsPoint(nextPos))
            {
                if (MoveMoveablePoint(unmoveableObjects, moveableObjects, nextPos, direction))
                {
                    return MoveCurrent();
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return MoveCurrent();
            }


            bool MoveCurrent()
            {
                var currentMoveableObject = moveableObjects.ValuePoints.First(p => p.Coordinate.Equals(moveableObjectPos));
                currentMoveableObject.Coordinate = nextPos;
                Grid[nextPos.X][nextPos.Y] = currentMoveableObject.Value;
                return true;
            }

        }

        private void MoveWorkerToNewPos(ValuePointCategory<char> workerValuePoints, Point nextPos)
        {
            var worker = workerValuePoints.ValuePoints.First();
            var prevPos = worker.Coordinate;
            worker.Coordinate = nextPos;
            Grid[nextPos.X][nextPos.Y] = worker.Value;
            Grid[prevPos.X][prevPos.Y] = '.';
        }

        private static Point GetNextPos(Point coordinate, Direction move)
        {
            return move switch
            {
                Direction.Left => new Point(coordinate.X - 1, coordinate.Y),
                Direction.Right => new Point(coordinate.X + 1, coordinate.Y),
                Direction.Up => new Point(coordinate.X, coordinate.Y + 1),
                Direction.Down => new Point(coordinate.X, coordinate.Y - 1),
                _ => throw new NotImplementedException(),
            };
        }

        protected override List<ValuePointCategory<char>> GetValuePointCharCategoriesInternal()
        {
            return [new ValuePointCategory<char>(UnmoveableCategoryName, unmoveableObstacles), new ValuePointCategory<char>(MoveableCategoryName, moveableObstacles),
            new ValuePointCategory<char>(WorkerCategoryName, workerChar)];
        }

        internal int CalculateCheckSum()
        {
            var points = GetValuePointCategoryByName(MoveableCategoryName).ValuePoints;
            //points = points.OrderByDescending(p => p.Coordinate.Y).ThenBy(p => p.Coordinate.X).ToList();
            return points.Sum(p => p.Coordinate.X + ((Grid.Length - 1 - p.Coordinate.Y) * 100));
        }
    }
}