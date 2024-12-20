using System.Drawing;

namespace aoc_2024.Solutions.Helper
{
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

        public int GetScore(int additionalScoreIfMoved = 0)
        {
            var score = GetSecondPointScore(VisitedPoints.First(), VisitedPoints[1], additionalScoreIfMoved);
            for (var pointIndex = 2; pointIndex < VisitedPoints.Count; pointIndex++)
            {
                var currentPointScore = GetPointScore(VisitedPoints[pointIndex], VisitedPoints[pointIndex - 2], additionalScoreIfMoved);
                score += currentPointScore;
            }
            return score;
        }

        private static int GetSecondPointScore(Point startPoint, Point secondPoint, int additionalScoreIfMoved)
        {
            if (secondPoint.X == startPoint.X + 1 && secondPoint.Y == startPoint.Y)
            {
                // east
                return 1;
            }
            if (secondPoint.X == startPoint.X - 1 && secondPoint.Y == startPoint.Y)
            {
                // west
                return 1 + 2 * additionalScoreIfMoved;
            }
            // north and south
            return 1 + additionalScoreIfMoved;
        }

        private static int GetPointScore(Point currentPoint, Point prevPrevPoint, int additionalScoreIfMoved)
        {
            if (prevPrevPoint.X != currentPoint.X && prevPrevPoint.Y != currentPoint.Y)
            {
                return 1 + additionalScoreIfMoved;
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