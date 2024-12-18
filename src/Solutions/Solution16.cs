using aoc_2024.Interfaces;
using aoc_2024.Solutions.Helper;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;

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
            var pathScore = maze.CalculateLowestPathScore(isTest);
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


    class MazeMap : MapBase
    {
        private readonly char[] _obstacleChars = ['#'];

        private readonly char[] _startPositionChars = ['S'];

        private readonly char[] _endPositionChars = ['E'];

        private readonly char[] _freeFieldChars = ['.'];

        public MazeMap(string inputData) : base(inputData)
        {
        }

        private const string Obstacles = "Obstalces";
        private const string Target = "EndPosition";
        private const string StartPosition = "StartPosition";
        private const string FreeFields = "FreeFields";

        protected override List<ValuePointCategory<char>> GetValuePointCharCategoriesInternal()
        {
            return [new ValuePointCategory<char>(Obstacles, _obstacleChars), new ValuePointCategory<char>(StartPosition, _startPositionChars),
            new ValuePointCategory<char>(Target, _endPositionChars), new ValuePointCategory<char>(FreeFields, _freeFieldChars)];
        }

        public int CalculateLowestPathScore(bool isTest)
        {
            // get startPos
            var startPos = GetValuePointCategoryByName(StartPosition).ValuePoints.First();
            var mazePaths = GetPathScoreBfs(startPos.Coordinate, new MazePath([]));
            var validMazePaths = mazePaths.Where(mp => mp.IsValidPath()).ToList();

            if (isTest)
            {
                var count = 0;
                foreach (var validMazePath in validMazePaths)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    var score = validMazePath.GetScore();
                    Console.WriteLine($"{++count}/{validMazePaths.Count} - Score {score}");
                    PrintColoredMap(validMazePath.GetPoints().ToDictionary(p => p, p => ConsoleColor.Yellow));
                }
            }
            var validMazePathsScores = validMazePaths.Select(mp => mp.GetScore()).ToList();
            return validMazePathsScores.Min();
        }


        static int staticCount = 0;
        const string imageSavePath = "C:\\Users\\JanSt\\OneDrive\\Dokumente\\AoC2024\\16";

        private List<MazePath> GetPathScoreBfs(Point currentPos, MazePath currentPath)
        {
            var currentPosValue = GetValueAtPos(currentPos);
            // check if End
            if (_endPositionChars.Contains(currentPosValue))
            {
                // if so, return current maze
                currentPath.AddPoint(currentPos);
                currentPath.SetFinished();
                return [currentPath];
            }
            // check if obstacle
            if (_obstacleChars.Contains(currentPosValue))
            {
                // we are obstacle, don't continue
                return [];
            }
            // if not, check if visited already
            if (currentPath.ContainsPoint(currentPos))
            {
                // return
                return [];
            }
            currentPath.AddPoint(currentPos);
            var dir = currentPath.GetPoints().ToDictionary(d => d, d => Color.Firebrick);
            Task.Factory.StartNew(() =>
            PrintColoredMapToFile(Path.Combine(imageSavePath, staticCount++ + "_way.png"), dir));
            // return paths for points around
            var pointsAround = currentPos.GetDirectPointsAround();
            var resultMazePaths = new List<MazePath>();
            foreach (var point in pointsAround)
            {
                var newMazePaths = GetPathScoreBfs(point, new MazePath(currentPath.GetPoints()));
                resultMazePaths.AddRange(newMazePaths);
            }
            return resultMazePaths;
        }

        internal void EliminateDeadEnds()
        {
            // get all FreeFields
            // get those who are sourounded by only 1 free field and else only obstacles
            // and with those that are sourrounded by only two free fields
            var run = 0;
            var sum = new List<(int run, long msNeeded)>();
            var sw = Stopwatch.StartNew();
            while (GetFreeFieldsSourroundingFields(1) is { } freeFields && freeFields.Any()) // TODO also add obstacles -> search for circles. Obstaccle with only free fields around
            {
                var currentCount = freeFields[1].Count;
                // make the first list obstacles
                AddFreeFieldsToObstacles(freeFields);
                sw.Stop();
                run++;
                Debug.WriteLine($"Finished run {run} with {currentCount} freeFields with only one other free field.");
                sum.Add((run, sw.ElapsedMilliseconds));

                Task.Factory.StartNew(() =>
                PrintColoredMapToFile(Path.Combine(imageSavePath, staticCount++ + ".png")));

                sw.Restart();
            }
            sw.Stop();
            var max = sum.MaxBy(s => s.msNeeded);
            var min = sum.MinBy(s => s.msNeeded);
            var average = sum.Average(s => s.msNeeded);
            Console.WriteLine($"Endet after {run} runs. Took max {max.msNeeded} ms at run {max.run}, min {min.msNeeded} ms at run {min.run} and average of {average} ms");
            // check those with only two fields and make those that now have one of those as obstacles to obstacles
            //AddPathFieldsToObstacles(pathFields);
        }

        private void AddFreeFieldsToObstacles(Dictionary<int, List<(ValuePoint<char> ValuePoint, List<ValuePoint<char>> SourroundingPoints)>> freeFields)
        {
            var containsObstalceFields = freeFields.TryGetValue(1, out var obstalceFields);
            if (!containsObstalceFields || obstalceFields == null)
            {
                return;
            }
            var obstacles = GetValuePointCategoryByName(Obstacles);
            var obstaclesToAdd = new ConcurrentBag<ValuePoint<char>>();
            Parallel.ForEach(obstalceFields, entry =>
            //foreach (var entry in obstalceFields!)
            {
                var currentValuePoint = entry.ValuePoint;
                obstaclesToAdd.Add(currentValuePoint);
                Grid[currentValuePoint.Coordinate.X][currentValuePoint.Coordinate.Y] = _obstacleChars.First();
            }
            );
            foreach (var obstalceToAdd in obstaclesToAdd)
            {
                obstacles.Add(obstalceToAdd);
            }
            var freeFieldsCategory = GetValuePointCategoryByName(FreeFields);
            freeFieldsCategory.Remove(obstalceFields.Select(o => o.ValuePoint).ToArray());
            freeFields.Remove(1);
        }
        private Dictionary<int, List<(ValuePoint<char> ValuePoint, List<ValuePoint<char>> SourroundingPoints)>> GetFreeFieldsSourroundingFields(params int[] countsToCareFor)
        {
            var freeFields = GetValuePointCategoryByName(FreeFields);
            var startFields = GetValuePointCategoryByName(StartPosition);
            var endFields = GetValuePointCategoryByName(Target);
            var freeFieldsSourrouncedByFields = new ConcurrentDictionary<int, List<(ValuePoint<char> ValuePoint, List<ValuePoint<char>>)>>();

            Parallel.ForEach(freeFields.ValuePoints, field =>
            {
                //foreach (var field in freeFields.ValuePoints)
                //{
                var sourroundingFields = field.Coordinate.GetDirectPointsAround().ToList();
                if (sourroundingFields.Any(sf => startFields.ContainsPoint(sf) || endFields.ContainsPoint(sf)))
                {
                    return;
                }
                sourroundingFields = sourroundingFields.Where(freeFields.ContainsPoint).ToList();
                AddToDict(field, sourroundingFields, freeFields, countsToCareFor);
                //}
            });
            return freeFieldsSourrouncedByFields.ToDictionary();


            void AddToDict(ValuePoint<char> fieldToAdd, List<Point> sourroundingFields, ValuePointCategory<char> freeFields, int[] countsToCareFor)
            {
                var sourroundingFieldValuePoints = sourroundingFields.Select(freeFields.GetValuePointByPoint).ToList();
                if (!countsToCareFor.Contains(sourroundingFieldValuePoints.Count))
                {
                    return;
                }
                var tupleToAdd = (fieldToAdd, sourroundingFieldValuePoints);
                if (freeFieldsSourrouncedByFields.TryGetValue(sourroundingFields.Count, out var listToAddTo))
                {
                    listToAddTo.Add(tupleToAdd);
                }
                else
                {
                    freeFieldsSourrouncedByFields.TryAdd(sourroundingFields.Count, [tupleToAdd]);
                }
            }
        }
    }

    class PathField<T>
    {
        public ValuePoint<T> Field { get; set; }

        public ValuePoint<T> OtherField { get; set; }

        public ValuePoint<T> AnotherField { get; set; }
    }

    class MazePath
    {
        private bool _isValidPath;

        private List<Point> VisitedPoints { get; set; }

        public MazePath(List<Point> points)
        {
            VisitedPoints = [.. points];
            _isValidPath = false;
        }

        public bool IsValidPath()
        {
            return _isValidPath;
        }

        public int GetScore()
        {
            var score = GetSecondPointScore(VisitedPoints.First(), VisitedPoints[1]);
            for (var pointIndex = 2; pointIndex < VisitedPoints.Count; pointIndex++)
            {
                var currentPointScore = GetPointScore(VisitedPoints[pointIndex], VisitedPoints[pointIndex - 2]);
                score += currentPointScore;
            }
            return score;
        }

        private static int GetSecondPointScore(Point startPoint, Point secondPoint)
        {
            if (secondPoint.X == startPoint.X + 1 && secondPoint.Y == startPoint.Y)
            {
                // east
                return 1;
            }
            if (secondPoint.X == startPoint.X - 1 && secondPoint.Y == startPoint.Y)
            {
                // west
                return 2001;
            }
            // north and south
            return 1001;
        }

        private static int GetPointScore(Point currentPoint, Point prevPrevPoint)
        {
            if (prevPrevPoint.X != currentPoint.X && prevPrevPoint.Y != currentPoint.Y)
            {
                return 1001;
            }
            return 1;
        }

        internal void AddPoint(Point coordinate)
        {
            VisitedPoints.Add(coordinate);
        }

        internal void SetFinished()
        {
            _isValidPath = true;
        }

        internal bool ContainsPoint(Point coordinate)
        {
            return VisitedPoints.Contains(coordinate);
        }

        internal List<Point> GetPoints()
        {
            return VisitedPoints;
        }
    }
}