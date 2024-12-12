using System.Drawing;

namespace aoc_2024.Solutions.Helper
{
    public static class PointExtensions
    {
        public static List<Point> GetDirectPointsAround(this Point point)
        {
            var points = new List<Point>();
            for (int value = -1; value < 2; value += 2)
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
