using aoc_2024.Interfaces;
using aoc_2024.Solutions.Helper;
using System.Diagnostics;
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

        public string RunPartB(string inputData)
        {
            var plantMap = new PlantMap(inputData);
            var regions = plantMap.GetRegions();
            //var calculationResults = regions.Values.SelectMany(v => v).Sum(r => r.CalculateFencePriceWithSidesCount());
            //return calculationResults.ToString();
            return regions.Sum(s => s.Value.Sum(r => r.CalculateFencePriceWithSidesCount())).ToString();
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
            return pointsAroundChar.Intersect(Points).Any();
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
            var sides = GetSidesCount();
            Debug.WriteLine($"{sides} sides could for group {Char}");
            return areaValue * sides;
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
            var cornerSum = Points.Sum(GetCorners);
            return cornerSum;
        }

        private int GetCorners(Point point)
        {
            var corners = 0;
            var containedPoints = GetContainedPoints(point.GetNamedDirectPointsAround());


            if (IsUpperLeftCorner(containedPoints))
            {
                corners++;
            }
            if (IsUpperRightCorner(containedPoints))
            {
                corners++;
            }
            if (IsLowerLeftCorner(containedPoints))
            {
                corners++;
            }
            if (IsLowerRightCorner(containedPoints))
            {
                corners++;
            }
            if (IsUpperLeftInnerCorner(containedPoints))
            {
                corners++;
            }
            if (isUpperRightInnerCorner(containedPoints))
            {
                corners++;
            }
            if (IsLowerLeftInnerCorner(containedPoints))
            {
                corners++;
            }
            if (IsLowerRightInnerCorner(containedPoints))
            {
                corners++;
            }

            return corners;

            (bool containsLeft, bool containsTop, bool containsRight, bool containsBottom) GetContainedPoints((Point Left, Point Top, Point Right, Point Bottom) value)
            {
                return (Points.Contains(value.Left), Points.Contains(value.Top), Points.Contains(value.Right), Points.Contains(value.Bottom));
            }

            bool IsUpperLeftCorner((bool containsLeft, bool containsTop, bool containsRight, bool containsBottom) containedPoints)
            {
                return !containedPoints.containsLeft && !containedPoints.containsTop;
            }

            bool IsUpperRightCorner((bool containsLeft, bool containsTop, bool containsRight, bool containsBottom) containedPoints)
            {
                return !containedPoints.containsTop && !containedPoints.containsRight;
            }

            bool IsLowerLeftCorner((bool containsLeft, bool containsTop, bool containsRight, bool containsBottom) containedPoints)
            {
                return !containedPoints.containsLeft && !containedPoints.containsBottom;
            }

            bool IsLowerRightCorner((bool containsLeft, bool containsTop, bool containsRight, bool containsBottom) containedPoints)
            {
                return !containedPoints.containsRight && !containedPoints.containsBottom;
            }

            bool IsUpperLeftInnerCorner((bool containsLeft, bool containsTop, bool containsRight, bool containsBottom) containedPoints)
            {
                return containedPoints.containsLeft && containedPoints.containsTop && !Points.Contains(point.NewPointFromVector(-1, 1));
            }

            bool isUpperRightInnerCorner((bool containsLeft, bool containsTop, bool containsRight, bool containsBottom) containedPoints)
            {
                return containedPoints.containsTop && containedPoints.containsRight && !Points.Contains(point.NewPointFromVector(1, 1));
            }

            bool IsLowerLeftInnerCorner((bool containsLeft, bool containsTop, bool containsRight, bool containsBottom) containedPoints)
            {
                return containedPoints.containsLeft && containedPoints.containsBottom && !Points.Contains(point.NewPointFromVector(-1, -1));
            }

            bool IsLowerRightInnerCorner((bool containsLeft, bool containsTop, bool containsRight, bool containsBottom) containedPoints)
            {
                return containedPoints.containsRight && containedPoints.containsBottom && !Points.Contains(point.NewPointFromVector(1, -1));
            }
        }
    }
}