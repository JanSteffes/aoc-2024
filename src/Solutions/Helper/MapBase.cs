using aoc_2024.SolutionUtils;
using System.Collections.ObjectModel;
using System.Drawing;

namespace aoc_2024.Solutions.Helper
{
    internal abstract class MapBase
    {
        protected char[][] Grid { get; private set; }

        private int MaxX { get; set; }

        private int MaxY { get; set; }

        protected List<ValuePoint<char>> ValuePoints { get; set; }

        public MapBase(string inputData)
        {
            ValuePoints = [];
            Grid = BuildCoordinateSystemFromStringAndFillValuePoints(inputData, GetValuePointChars());
            MaxY = MaxX = Grid.Length;
        }

        private char[][] BuildCoordinateSystemFromStringAndFillValuePoints(string inputData, ReadOnlyCollection<char>? obstacleChars)
        {
            var lines = ParseUtils.ParseIntoLines(inputData);
            var length = lines.Length;
            var coordinateSystem = new char[length][];
            for (int column = 0; column < length; column++)
            {
                coordinateSystem[column] = new char[length];
                for (int row = 0; row < length; row++)
                {
                    var charToSet = lines[length - 1 - row][column];
                    if (obstacleChars?.Contains(charToSet) ?? false)
                    {
                        ValuePoints.Add(new ValuePoint<char>(charToSet, new Point(column, row)));
                    }
                    coordinateSystem[column][row] = charToSet;
                }
            }
            return coordinateSystem;
        }

        public void AddValuePoint(ValuePoint<char> valuePoint)
        {
            ValuePoints.Add(valuePoint);
        }

        public bool IsInMap(Point position)
        {
            return position.Y < MaxY && position.X < MaxX && position.Y >= 0 && position.X >= 0;
        }

        protected virtual ReadOnlyCollection<char>? GetValuePointChars()
        {
            return default;
        }
    }

    internal class ValuePoint<T>
    {
        public T Value { get; set; }

        public Point Coordinate { get; private set; }

        public ValuePoint(T value, Point coordinate)
        {
            Value = value;
            Coordinate = coordinate;
        }

    }
}