using aoc_2024.Interfaces;
using aoc_2024.Solutions.Helper;
using System.Drawing;

namespace aoc_2024.Solutions
{
    public class Solution12 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var plantMap = new PlantMap(inputData);
            var regions = plantMap.GetRegions();
            //var colorsForRegions = GetColorsForPointsInRegions(regions);
            //plantMap.PrintMap(colorsForRegions);
            return regions.Sum(s => s.Value.Sum(r => r.CalculateFencePriceWithPerimeter())).ToString();

        }

        private Dictionary<Point, ConsoleColor>? GetColorsForPointsInRegions(Dictionary<char, List<Region>> regions)
        {
            var resultDict = new Dictionary<Point, ConsoleColor>();
            ConsoleColor[] consoleColors = [.. Enum.GetValues<ConsoleColor>().Except([ConsoleColor.Black])];
            foreach (var region in regions.Values.SelectMany(q => q))
            {
                var color = Random.Shared.GetItems(consoleColors, 1)[0];
                consoleColors = consoleColors.Except([color]).ToArray();
                if (consoleColors.Length == 0)
                {
                    consoleColors = [.. Enum.GetValues<ConsoleColor>().Except([ConsoleColor.Black])];
                }
                foreach (var point in region.Points)
                {
                    resultDict.Add(point, color);
                }
            }
            return resultDict;
        }

        public string RunPartB(string inputData)
        {
            var plantMap = new PlantMap(inputData);
            var regions = plantMap.GetRegions();
            return regions.Sum(s => s.Value.Sum(r => r.CalculateFencePriceWithPerimeter())).ToString();
        }
    }

    class PlantMap : MapBase
    {
        public PlantMap(string inputData) : base(inputData)
        {

        }

        internal Dictionary<char, List<Region>> GetRegions()
        {
            var regions = new Dictionary<char, List<Region>>();
            for (var y = Grid.Length - 1; y >= 0; y--)
            {
                for (var x = 0; x < Grid[y].Length; x++)
                {
                    var currentPos = new Point(x, y);
                    var currentChar = GetValueAtPos(currentPos);
                    if (regions.TryGetValue(currentChar, out var regionsForChar))
                    {
                        var pointsAroundChar = GetValidPointsAroundPoint(currentPos);
                        var matchingRegions = regionsForChar.Where(r => r.ContainsAnyPoint(pointsAroundChar)).ToList();
                        switch (matchingRegions.Count)
                        {
                            case 0:
                                regionsForChar.Add(new Region(currentPos, currentChar));
                                break;
                            case 1:
                                matchingRegions[0].Add(currentPos);
                                break;
                            default:
                                var regionsToKill = matchingRegions.Skip(1).ToArray();
                                matchingRegions[0].IncludeRegions(regionsToKill);
                                matchingRegions[0].Add(currentPos);
                                regionsForChar.RemoveAll(regionsToKill.Contains);
                                break;
                        }

                    }
                    else
                    {
                        regions.Add(currentChar, [new Region(currentPos, currentChar)]);
                    }
                }
            }
            return regions;
        }

        private List<Point> GetValidPointsAroundPoint(Point startingPoint)
        {
            var points = new List<Point>();
            foreach (var pointAround in startingPoint.GetDirectPointsAround())
            {
                TryAddPoint(points, pointAround);
            }
            return points;
        }

        private void TryAddPoint(List<Point> points, Point point)
        {
            if (!points.Contains(point) && IsInMap(point))
            {
                points.Add(point);
            }
        }
    }

    class Region
    {

        public char Char { get; private set; }

        public HashSet<Point> Points { get; set; }

        public Region(Point currentPos, char charForRegion)
        {
            Points = [currentPos];
            Char = charForRegion;
        }

        public bool ContainsAnyPoint(List<Point> pointsAroundChar)
        {
            return pointsAroundChar.Intersect(Points).Count() > 0;
        }

        public void IncludeRegions(IEnumerable<Region> regions)
        {
            foreach (var region in regions)
            {
                foreach (var point in region.Points)
                {
                    Points.Add(point);
                };
            }
        }

        public void Add(Point currentPos)
        {
            Points.Add(currentPos);
        }

        public int CalculateFencePriceWithPerimeter()
        {
            var areaValue = GetAreaValue();
            var perimeter = GetPerimeterValue();
            return areaValue * perimeter;
        }

        public int CalculateFencePriceWithSidesCount()
        {
            var areaValue = GetAreaValue();
            var perimeter = GetSidesCount();
            return areaValue * perimeter;
        }

        private int GetAreaValue()
        {
            return Points.Count;
        }

        private int GetPerimeterValue()
        {
            return Points.Sum(p => p.GetDirectPointsAround().Except(Points).Count());
        }

        private int GetSidesCount()
        {
            throw new NotImplementedException();
        }
    }
}