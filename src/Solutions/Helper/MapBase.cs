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

        public void PrintColoredMap(IDictionary<Point, ConsoleColor>? customColors = default)
        {
            Thread.Sleep(500);
            var colorsForCategories = GetConsoleColorsForPointsInCategories(GetValuePointCategories(), [ConsoleColor.Green, ConsoleColor.Red, ConsoleColor.Blue, ConsoleColor.Magenta, ConsoleColor.Yellow, ConsoleColor.Cyan, ConsoleColor.Gray]);
            PrintMap(colorsForCategories, customColors);
        }


        public void PrintColoredMapToFile(string filePath, IDictionary<Point, Color>? customColors = default)
        {
            var colorsForCategories = GetConsoleColorsForPointsInCategories(GetValuePointCategories(), [Color.Green, Color.Red, Color.Blue, Color.Yellow, Color.Cyan, Color.Magenta, Color.LightGray]);
            PrintToImage(filePath, colorsForCategories, customColors ?? new Dictionary<Point, Color>());
        }

        void PrintToImage(string filePath, IDictionary<Point, Color> colorValues, IDictionary<Point, Color> customColors)
        {
            var bitmap = new Bitmap(MaxX, MaxY);
            for (int y = 0; y < MaxY; y++)
            {
                for (int x = 0; x < MaxX; x++)
                {
                    var current = new Point(x, y);
                    if (!customColors.TryGetValue(current, out var color))
                    {
                        colorValues.TryGetValue(current, out color);
                    }
                    bitmap.SetPixel(x, y, color);

                }
            }
            var newBitmap = new Bitmap(bitmap, bitmap.Size.Width * 4, bitmap.Size.Height * 4);
            //Task.Factory.StartNew(() =>
            //{
            newBitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            newBitmap.Dispose();
            bitmap.Dispose();
            //});
        }

        private Dictionary<Point, T> GetConsoleColorsForPointsInCategories<T>(List<ValuePointCategory<char>> list, T[] valuesToTakeFrom)
        {
            var resultDict = new Dictionary<Point, T>();

            var colorIndex = 0;
            foreach (var category in list)
            {
                if (valuesToTakeFrom.Length == colorIndex)
                {
                    colorIndex = 0;
                }
                var color = valuesToTakeFrom[colorIndex++];
                foreach (var point in category.ValuePoints)
                {
                    resultDict.Add(point.Coordinate, color);
                }
            }
            return resultDict;
        }

        public void PrintMap(Dictionary<Point, ConsoleColor>? colorsForPoints = null, IDictionary<Point, ConsoleColor>? customColors = null)
        {
            for (var y = MaxY - 1; y >= 0; y--)
            {
                for (var x = 0; x < MaxX; x++)
                {
                    PrintColoredPoint(colorsForPoints, customColors, x, y);
                }
                Console.WriteLine();
            }
        }

        private void PrintColoredPoint(Dictionary<Point, ConsoleColor>? colorsForPoints, IDictionary<Point, ConsoleColor>? customColors, int x, int y)
        {
            var currentPoint = new Point(x, y);
            var prevColor = Console.ForegroundColor;
            if (customColors?.TryGetValue(currentPoint, out ConsoleColor color) ?? false)
            {
                Console.ForegroundColor = color;
            }
            else if (colorsForPoints?.TryGetValue(currentPoint, out color) ?? false)
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

        public T GetValueByPoint(Point point)
        {
            return GetValuePointByPoint(point).Value;
        }

        internal ValuePoint<T> GetValuePointByPoint(Point point)
        {
            return ValuePoints.First(v => v.EqualsCoordinate(point));
        }

        internal void Remove(params ValuePoint<T>[] valuePointsToRemove)
        {
            foreach (var valuePointToRemove in valuePointsToRemove)
            {
                ValuePoints.Remove(valuePointToRemove);
            }
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