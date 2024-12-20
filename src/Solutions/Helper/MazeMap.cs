using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;

namespace aoc_2024.Solutions.Helper
{
    class MazeMap : MapBase
    {
        private readonly char[] _obstacleChars = ['#'];

        private readonly char[] _startPositionChars = ['S'];

        private readonly char[] _endPositionChars = ['E'];

        private readonly char[] _freeFieldChars = ['.'];

        public MazeMap(string inputData) : base(inputData)
        {
        }

        private const string ObstaclesCategoryName = "Obstalces";
        private const string TargetCategoryName = "EndPosition";
        private const string StartPositionCategoryName = "StartPosition";
        private const string FreeFieldsCategoryName = "FreeFields";

        protected override List<ValuePointCategory<char>> GetValuePointCharCategoriesInternal()
        {
            return [new ValuePointCategory<char>(ObstaclesCategoryName, _obstacleChars), new ValuePointCategory<char>(StartPositionCategoryName, _startPositionChars),
            new ValuePointCategory<char>(TargetCategoryName, _endPositionChars), new ValuePointCategory<char>(FreeFieldsCategoryName, _freeFieldChars)];
        }

        public int CalculateLowestPathScore(bool isTest, int additionalScore)
        {
            // get startPos
            var startPos = GetValuePointCategoryByName(StartPositionCategoryName).ValuePoints.First();
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
                    var score = validMazePath.GetScore(additionalScore);
                    Console.WriteLine($"{++count}/{validMazePaths.Count} - Score {score}");
                    PrintColoredMap(validMazePath.GetPoints().ToDictionary(p => p, p => ConsoleColor.Yellow));
                }
            }
            var validMazePathsScores = validMazePaths.Select(mp => mp.GetScore(additionalScore)).ToList();
            return validMazePathsScores.Min();
        }


        static int staticCount = 0;
        const string imageSavePath = "C:\\Users\\JanSt\\OneDrive\\Dokumente\\AoC2024\\16";

        protected List<MazePath> GetPathScoreBfs(Point currentPos, MazePath currentPath)
        {
            if (!IsInMap(currentPos))
            {
                return [];
            }
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
            //PrintColoredMapToFileInBackground(Path.Combine(imageSavePath, staticCount++ + "_way.png"), dir);
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

                //PrintColoredMapToFileInBackground(Path.Combine(imageSavePath, staticCount++ + ".png"));

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
            var obstacles = GetValuePointCategoryByName(ObstaclesCategoryName);
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
            var freeFieldsCategory = GetValuePointCategoryByName(FreeFieldsCategoryName);
            freeFieldsCategory.Remove(obstalceFields.Select(o => o.ValuePoint).ToArray());
            freeFields.Remove(1);
        }
        private Dictionary<int, List<(ValuePoint<char> ValuePoint, List<ValuePoint<char>> SourroundingPoints)>> GetFreeFieldsSourroundingFields(params int[] countsToCareFor)
        {
            var freeFields = GetValuePointCategoryByName(FreeFieldsCategoryName);
            var startFields = GetValuePointCategoryByName(StartPositionCategoryName);
            var endFields = GetValuePointCategoryByName(TargetCategoryName);
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
}