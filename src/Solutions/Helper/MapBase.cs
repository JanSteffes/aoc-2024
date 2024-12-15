using aoc_2024.SolutionUtils;
using System.Drawing;

namespace aoc_2024.Solutions.Helper
{
    internal abstract class MapBase
    {
        protected char[][] Grid { get; private set; }

        protected int MaxX { get; set; }

        protected int MaxY { get; set; }

        protected List<ValuePointCategory<char>> ValuePointCategories { get; set; }

        public MapBase(string inputData)
        {
            ValuePointCategories = GetValuePointCharCategoriesInternal();
            Grid = BuildCoordinateSystemFromStringAndFillValuePoints(inputData);
            MaxX = Grid.Length;
            MaxY = Grid[0].Length;
        }

        private char[][] BuildCoordinateSystemFromStringAndFillValuePoints(string inputData)
        {
            var lines = ParseUtils.ParseIntoLines(inputData).Reverse().ToArray();
            var height = lines.Length;
            var coordinateSystem = new char[lines.First().Length][];
            for (int y = 0; y < height; y++)
            {
                var currentRow = lines[y];
                for (int x = 0; x < currentRow.Length; x++)
                {
                    if (coordinateSystem[x] == null)
                    {
                        coordinateSystem[x] = new char[height];
                    }
                    var charToSet = currentRow[x];
                    var category = ValuePointCategories.FirstOrDefault(v => v.ContainsValue(charToSet));
                    if (category != default)
                    {
                        category.Add(new ValuePoint<char>(charToSet, new Point(x, y)));
                    }
                    coordinateSystem[x][y] = charToSet;
                }
            }
            return coordinateSystem;
        }

        public void PrintMap(Dictionary<Point, ConsoleColor>? colorsForPoints = null)
        {
            for (var y = MaxY - 1; y >= 0; y--)
            {
                for (var x = 0; x < MaxX; x++)
                {
                    PrintColoredPoint(colorsForPoints, x, y);
                }
                Console.WriteLine();
            }
        }

        private void PrintColoredPoint(Dictionary<Point, ConsoleColor>? colorsForPoints, int x, int y)
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