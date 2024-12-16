using aoc_2024.Interfaces;
using aoc_2024.Solutions.Helper;
using System.Drawing;

namespace aoc_2024.Solutions
{
    public class Solution16 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var isTest = false;
            var maze = new MazeMap(inputData);
            if (isTest)
            {
                maze.PrintColoredMap();
            }
            var pathScore = maze.CalculateLowestPathScore(isTest);
            return pathScore.ToString();
        }

        public string RunPartB(string inputData)
        {
            throw new NotImplementedException();
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

        protected override List<ValuePointCategory<char>> GetValuePointCharCategoriesInternal()
        {
            return [new ValuePointCategory<char>("Obstalces", _obstacleChars), new ValuePointCategory<char>("StartPosition", _startPositionChars),
            new ValuePointCategory<char>("EndPosition", _endPositionChars), new ValuePointCategory<char>("FreeFields", _freeFieldChars)];
        }

        public int CalculateLowestPathScore(bool isTest)
        {
            // get startPos
            var startPos = GetValuePointCategoryByName("StartPosition").ValuePoints.First();
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
            //var dir = currentPath.GetPoints().ToDictionary(d => d, d => Color.Yellow);
            //Task.Factory.StartNew(() =>
            //PrintColoredMapToFile(Path.Combine(imageSavePath, staticCount++ + ".png"), dir));
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