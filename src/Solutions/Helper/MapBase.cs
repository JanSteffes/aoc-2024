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
            ValuePointCategories = GetValuePointCharCategoriesInternal();
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
                    var category = ValuePointCategories.FirstOrDefault(v => v.ContainsValue(charToSet));
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
                var currentRowLength = Grid[y].Length;
                for (var x = 0; x < currentRowLength; x++)
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
            var categoryToAdd = ValuePointCategories.FirstOrDefault(v => v.ContainsValue(valuePoint.Value));
            if (categoryToAdd != default)
            {
                categoryToAdd.Add(valuePoint);
            }
        }

        public bool IsInMap(Point position)
        {
            return position.Y < MaxY && position.X < MaxX && position.Y >= 0 && position.X >= 0;
        }

        public List<ValuePointCategory<char>> GetValuePointCategories()
        {
            return ValuePointCategories;
        }

        protected ValuePointCategory<char> GetValuePointCategoryByName(string name)
        {
            return ValuePointCategories.First(v => v.Name == name);
        }


        protected virtual List<ValuePointCategory<char>> GetValuePointCharCategoriesInternal()
        {
            return [];
        }
    }

    internal class ValuePointCategory<T>
    {
        public string Name { get; set; }

        public T[] CategoryValues { get; set; }

        public List<ValuePoint<T>> ValuePoints { get; set; }

        public ValuePointCategory(string name, T[] categoryValues)
        {
            Name = name;
            CategoryValues = categoryValues;
            ValuePoints = [];
        }

        public bool ContainsValue(T valueToContains)
        {
            return CategoryValues.Contains(valueToContains);
        }

        public bool ContainsPoint(Point point)
        {
            return ValuePoints.Any(v => v.EqualsCoordinate(point));
        }

        public void Add(ValuePoint<T> value)
        {
            ValuePoints.Add(value);
        }

        public T GetValueByPoint(Point moveableObjectPos)
        {
            return ValuePoints.First(v => v.EqualsCoordinate(moveableObjectPos)).Value;
        }
    }

    internal class ValuePoint<T>
    {
        public T Value { get; set; }

        public Point Coordinate { get; set; }

        public ValuePoint(T value, Point coordinate)
        {
            Value = value;
            Coordinate = coordinate;
        }

        public virtual bool EqualsCoordinate(Point point)
        {
            return Coordinate.Equals(point);
        }

        public override string ToString()
        {
            return $"X: {Coordinate.X}/Y: {Coordinate.Y}: {Value}";
        }

    }
}