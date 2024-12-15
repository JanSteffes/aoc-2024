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
            ParseLines(inputData, out List<string> mapLines, out List<string> moveLines);

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

        public string RunPartB(string inputData)
        {
            ParseLines(inputData, out List<string> mapLines, out List<string> moveLines);

            var mapInputString = string.Join(Environment.NewLine, mapLines);
            mapInputString = mapInputString.Replace("#", "##").Replace("O", "[]").Replace(".", "..").Replace("@", "@.");
            var map = new WarehouseScaledMap(mapInputString);

            PrintColoredMap(map);

            var moveSets = ParseDirections(string.Join(string.Empty, moveLines));
            foreach (var move in moveSets)
            {
                map.TryMoveWorker(move);
                PrintColoredMap(map);
            }

            var checkSum = map.CalculateCheckSum();

            return checkSum.ToString();
        }

        private List<Direction> ParseDirections(string moveset)
        {
            return moveset.Select(c => (Direction)Enum.ToObject(typeof(Direction), c)).ToList();
        }

        private static void ParseLines(string inputData, out List<string> mapLines, out List<string> moveLines)
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
    }

    class WarehouseScaledMap : WarehouseMap
    {
        private readonly char[] moveableObstacles = ['[', ']'];
        protected override char[] MoveableObstacles { get => moveableObstacles; }

        public WarehouseScaledMap(string inputData) : base(inputData)
        {
        }

        protected override bool CanMoveMoveablePoint(ValuePointCategory<char> unmoveableObject, ValuePointCategory<char> moveableObjects, Point moveableObjectPos, Direction move)
        {
            var nextPos = GetNextPos(moveableObjectPos, move);
            var secondNextPos = GetSecondPart(moveableObjects, moveableObjectPos, move);
            if (unmoveableObject.ContainsPoint(nextPos) || unmoveableObject.ContainsPoint(secondNextPos))
            {
                return false;
            }
            if (moveableObjects.ContainsPoint(nextPos) || moveableObjects.ContainsPoint(secondNextPos))
            {
                return new Point[] { nextPos, secondNextPos }.Where(moveableObjects.ContainsPoint).All(mp => CanMoveMoveablePoint(unmoveableObject, moveableObjects, mp, move));
            }
            return true;
        }

        private static Point GetSecondPart(ValuePointCategory<char> moveableObjects, Point moveableObjectPos, Direction move)
        {
            var firstChar = moveableObjects.GetValueByPoint(moveableObjectPos);
            var secondNextPos = firstChar switch
            {
                '[' => GetNextPos(moveableObjectPos.NewPointFromVector(1, 0), move),
                ']' => GetNextPos(moveableObjectPos.NewPointFromVector(-1, 0), move),
                _ => throw new ArgumentException($"Should be moveable but is not? At '{moveableObjectPos}' during move {move}!"),
            };
            return secondNextPos;
        }

        protected override void MoveMoveablePoint(ValuePointCategory<char> moveableObjects, Point moveableObjectPos, Direction direction)
        {
            var nextPos = GetNextPos(moveableObjectPos, direction);
            var secondPos = GetSecondPart(moveableObjects, moveableObjectPos, direction);
            if (moveableObjects.ContainsPoint(nextPos) || moveableObjects.ContainsPoint(secondPos))
            {
                MoveMoveablePoint(moveableObjects, moveableObjectPos, direction);
            }

            var currentToMove = moveableObjects.ValuePoints.First(p => p.Coordinate.Equals(moveableObjectPos));
            MoveCurrent(currentToMove, nextPos);
            var secondToMove = moveableObjects.ValuePoints.First(p => p.Coordinate.Equals(secondPos));
            MoveCurrent(secondToMove, secondPos);
        }

        protected override List<ValuePoint<char>> GetPointsForChecksum()
        {
            // foreach moveable, get closest to edge
            // for y its the same, but x might be one less
            // just get all where char = [
            return GetValuePointCategoryByName(MoveableCategoryName).ValuePoints.Where(p => p.Value == '[').ToList();
        }
    }


    class WarehouseMap : MapBase
    {
        protected const string UnmoveableCategoryName = "Unmoveable";
        protected const string MoveableCategoryName = "Moveable";
        protected const string WorkerCategoryName = "Worker";
        private readonly char[] unmoveableObstacles = ['#'];

        private readonly char[] moveableObstacles = ['O'];
        protected virtual char[] MoveableObstacles { get => moveableObstacles; }

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
                if (CanMoveMoveablePoint(unmoveableObject, moveableObjects, nextPos, move))
                {
                    MoveMoveablePoint(moveableObjects, nextPos, move);
                    MoveWorkerToNewPos(workerValuePoints, nextPos);
                }
                return;
            }
            // if newPos is free, move
            MoveWorkerToNewPos(workerValuePoints, nextPos);
        }

        protected virtual bool CanMoveMoveablePoint(ValuePointCategory<char> unmoveableObject, ValuePointCategory<char> moveableObjects, Point moveableObjectPos, Direction move)
        {
            var nextPos = GetNextPos(moveableObjectPos, move);
            if (unmoveableObject.ContainsPoint(nextPos))
            {
                return false;
            }
            if (moveableObjects.ContainsPoint(nextPos))
            {
                return CanMoveMoveablePoint(unmoveableObject, moveableObjects, nextPos, move);
            }
            return true;
        }

        protected virtual void MoveMoveablePoint(ValuePointCategory<char> moveableObjects, Point moveableObjectPos, Direction direction)
        {
            var nextPos = GetNextPos(moveableObjectPos, direction);
            if (moveableObjects.ContainsPoint(nextPos))
            {
                MoveMoveablePoint(moveableObjects, nextPos, direction);
            }
            var currentToMove = moveableObjects.ValuePoints.First(p => p.Coordinate.Equals(moveableObjectPos));
            MoveCurrent(currentToMove, nextPos);
        }

        protected bool MoveCurrent(ValuePoint<char> currentMoveableObject, Point nextPos)
        {
            var prevPos = currentMoveableObject.Coordinate;
            currentMoveableObject.Coordinate = nextPos;
            Grid[nextPos.X][nextPos.Y] = currentMoveableObject.Value;
            Grid[prevPos.X][prevPos.Y] = '.';
            return true;
        }


        private void MoveWorkerToNewPos(ValuePointCategory<char> workerValuePoints, Point nextPos)
        {
            var worker = workerValuePoints.ValuePoints.First();
            var prevPos = worker.Coordinate;
            worker.Coordinate = nextPos;
            Grid[nextPos.X][nextPos.Y] = worker.Value;
            Grid[prevPos.X][prevPos.Y] = '.';
        }

        protected static Point GetNextPos(Point coordinate, Direction move)
        {
            return move switch
            {
                Direction.Left => coordinate.NewPointFromVector(-1, 0),
                Direction.Right => coordinate.NewPointFromVector(1, 0),
                Direction.Up => coordinate.NewPointFromVector(0, 1),
                Direction.Down => coordinate.NewPointFromVector(0, -1),
                _ => throw new NotImplementedException(),
            };
        }

        protected override List<ValuePointCategory<char>> GetValuePointCharCategoriesInternal()
        {
            return [new ValuePointCategory<char>(UnmoveableCategoryName, unmoveableObstacles), new ValuePointCategory<char>(MoveableCategoryName, MoveableObstacles),
            new ValuePointCategory<char>(WorkerCategoryName, workerChar)];
        }

        public int CalculateCheckSum()
        {
            var points = GetPointsForChecksum();
            return points.Sum(p => p.Coordinate.X + ((Grid.Length - 1 - p.Coordinate.Y) * 100));
        }

        protected virtual List<ValuePoint<char>> GetPointsForChecksum()
        {
            return GetValuePointCategoryByName(MoveableCategoryName).ValuePoints;
        }
    }
}