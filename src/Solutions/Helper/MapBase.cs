using aoc_2024.SolutionUtils;
using System.Drawing;

namespace aoc_2024.Solutions.Helper
{
    internal abstract class MapBase
    {
        protected char[][] Grid { get; private set; }

        private int MaxX { get; set; }

        private int MaxY { get; set; }

        protected List<ValuePointCategory<char>> ValuePointCategories { get; set; }

        public MapBase(string inputData)
        {
            ValuePointCategories = GetValuePointCharCategories();
            Grid = BuildCoordinateSystemFromStringAndFillValuePoints(inputData);
            MaxY = MaxX = Grid.Length;
        }

        private char[][] BuildCoordinateSystemFromStringAndFillValuePoints(string inputData)
        {
            var lines = ParseUtils.ParseIntoLines(inputData);
            var length = lines.Length;
            var coordinateSystem = new char[length][];
            for (int row = 0; row < length; row++)
            {
                coordinateSystem[row] = new char[length];
                for (int column = 0; column < length; column++)
                {
                    var charToSet = lines[length - 1 - column][row];
                    var category = ValuePointCategories.FirstOrDefault(v => v.ContainsChar(charToSet));
                    if (category != default)
                    {
                        category.Add(new ValuePoint<char>(charToSet, new Point(row, column)));
                    }
                    coordinateSystem[row][column] = charToSet;
                }
            }
            return coordinateSystem;
        }

        public void PrintMap(Dictionary<Point, ConsoleColor>? colorsForPoints = null)
        {
            for (var y = Grid.Length - 1; y >= 0; y--)
            {
                for (var x = 0; x < Grid.Length; x++)
                {
                    var currentPoint = new Point(x, y);
                    var prevColor = Console.ForegroundColor;
                    if (colorsForPoints?.TryGetValue(currentPoint, out ConsoleColor color) ?? false)
                    {
                        Console.ForegroundColor = color;
                    }
                    Console.Write(GetValueAtPos(currentPoint));
                    Console.ForegroundColor = prevColor;
                }
                Console.WriteLine();
            }
        }
        public char GetValueAtPos(Point startingPoint)
        {
            return GetValueAtPos(startingPoint.X, startingPoint.Y);
        }
        public char GetValueAtPos(int x, int y)
        {
            return Grid[x][y];
        }

        public void AddValuePoint(ValuePoint<char> valuePoint)
        {
            var categoryToAdd = ValuePointCategories.FirstOrDefault(v => v.ContainsChar(valuePoint.Value));
            if (categoryToAdd != default)
            {
                categoryToAdd.Add(valuePoint);
            }
        }

        public bool IsInMap(Point position)
        {
            return position.Y < MaxY && position.X < MaxX && position.Y >= 0 && position.X >= 0;
        }

        protected virtual List<ValuePointCategory<char>> GetValuePointCharCategories()
        {
            return [];
        }
    }

    internal class ValuePointCategory<T>
    {
        public string Name { get; set; }

        public char[] CategoryValues { get; set; }

        public List<ValuePoint<T>> ValuePoints { get; set; }

        public ValuePointCategory(string name, char[] categoryValues)
        {
            Name = name;
            CategoryValues = categoryValues;
            ValuePoints = [];
        }

        public bool ContainsChar(char character)
        {
            return CategoryValues.Contains(character);
        }

        public void Add(ValuePoint<T> value)
        {
            ValuePoints.Add(value);
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