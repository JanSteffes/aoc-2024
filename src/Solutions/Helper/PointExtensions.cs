using System.Drawing;

namespace aoc_2024.Solutions.Helper
{
    public static class PointExtensions
    {
        public static Point GetOffSetAsPoint(this Point point, Point pointToGetVectorFrom)
        {
            return new Point(pointToGetVectorFrom.X - point.X, pointToGetVectorFrom.Y - point.Y);
        }

        /// <summary>
        /// Overload of <see cref="NewPointFromVector(Point, Point)"/>
        /// </summary>
        /// <param name="point"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Point NewPointFromVector(this Point point, Point vector)
        {
            return NewPointFromVector(point, vector.X, vector.Y);
        }

        public static Point NewPointFromVector(this Point point, int x, int y)
        {
            return new Point(point.X + x, point.Y + y);
        }

        public static (Point Left, Point Top, Point Right, Point Bottom) GetNamedDirectPointsAround(this Point point)
        {
            var points = GetDirectPointsAround(point);
            return (points[0], points[3], points[2], points[1]);
        }


        /// <summary>
        /// Returns horizontal and vertical points for point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static List<Point> GetDirectPointsAround(this Point point)
        {
            return InternalGetPointsByIncrement(point, 2);
        }

        /// <summary>
        /// Returns horizontal, vertical and diagonal points for point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static List<Point> GetPointsAround(this Point point)
        {
            return InternalGetPointsByIncrement(point, 1);
        }

        private static List<Point> InternalGetPointsByIncrement(Point point, int increment)
        {
            var points = new List<Point>();
            for (int value = -1; value < 2; value += increment)
            {
                // don't include points value
                if (value == 0)
                {
                    continue;
                }
                points.Add(new Point(point.X + value, point.Y));
                points.Add(new Point(point.X, point.Y + value));
            }
            return points;
        }
    }
}
