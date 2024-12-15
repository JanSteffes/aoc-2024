using aoc_2024.Interfaces;
using aoc_2024.Solutions.Helper;
using System.Drawing;

namespace aoc_2024.Solutions
{
    public class Solution08 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var map = new AntennaMap(inputData);
            var antennaPositionsByFrequency = map.GetAntennaPositionsByFrequency();
            var antinodes = new Dictionary<char, List<Point>>();
            foreach (var antennaPositionGroup in antennaPositionsByFrequency)
            {
                var currentAntiNodes = GetAntinodesForAntennaGroupA(map, antennaPositionsByFrequency, antennaPositionGroup);
                antinodes.Add(antennaPositionGroup.Key, currentAntiNodes);
            }
            var distinctAntinodes = antinodes.SelectMany(a => a.Value).Distinct().ToList();
            // 426
            return distinctAntinodes.Count.ToString();
        }


        public string RunPartB(string inputData)
        {
            var map = new AntennaMap(inputData);
            var antennaPositionsByFrequency = map.GetAntennaPositionsByFrequency();
            var antinodes = new Dictionary<char, List<Point>>();
            foreach (var antennaPositionGroup in antennaPositionsByFrequency)
            {
                var currentAntiNodes = GetAntinodesForAntennaGroupB(map, antennaPositionsByFrequency, antennaPositionGroup);
                antinodes.Add(antennaPositionGroup.Key, currentAntiNodes);
            }
            var distinctAntinodes = antinodes.SelectMany(a => a.Value).Distinct().ToList();
            // 1359
            return distinctAntinodes.Count.ToString();
        }

        private static List<Point> GetAntinodesForAntennaGroupA(AntennaMap map, IDictionary<char, List<Point>> antennaPositionsByFrequency, KeyValuePair<char, List<Point>> antennaPositionGroup)
        {
            var currentAntiNodes = new List<Point>();
            var currentChar = antennaPositionGroup.Key;
            var positions = antennaPositionGroup.Value;
            var otherAntennaPositionGroups = antennaPositionsByFrequency.Where(e => e.Key != currentChar).ToList();
            foreach (var antennaPosition in positions)
            {
                foreach (var otherPosition in positions.Except([antennaPosition]))
                {
                    // calculate offset from my position to the other
                    var (OffsetX, OffsetY) = (
                        otherPosition.X - antennaPosition.X,
                        otherPosition.Y - antennaPosition.Y
                    );
                    var antiNodePosition = new Point(otherPosition.X + OffsetX,
                        otherPosition.Y + OffsetY);
                    if (map.IsInMap(antiNodePosition))
                    {
                        currentAntiNodes.Add(antiNodePosition);
                    }
                }
            }
            return currentAntiNodes;
        }

        private static List<Point> GetAntinodesForAntennaGroupB(AntennaMap map, IDictionary<char, List<Point>> antennaPositionsByFrequency, KeyValuePair<char, List<Point>> antennaPositionGroup)
        {
            var currentAntiNodes = new List<Point>(antennaPositionGroup.Value);
            var currentChar = antennaPositionGroup.Key;
            var positions = antennaPositionGroup.Value;
            var otherAntennaPositionGroups = antennaPositionsByFrequency.Where(e => e.Key != currentChar).ToList();
            foreach (var antennaPosition in positions)
            {
                foreach (var otherPosition in positions.Except([antennaPosition]))
                {
                    // calculate offset from my position to the other
                    var (OffsetX, OffsetY) = (
                        otherPosition.X - antennaPosition.X,
                        otherPosition.Y - antennaPosition.Y
                    );
                    var antiNodePosition = new Point(otherPosition.X + OffsetX,
                        otherPosition.Y + OffsetY);
                    while (map.IsInMap(antiNodePosition))
                    {
                        currentAntiNodes.Add(antiNodePosition);
                        antiNodePosition = new Point(antiNodePosition.X + OffsetX,
                        antiNodePosition.Y + OffsetY);
                    }
                }
            }
            return currentAntiNodes;
        }
    }

    internal class AntennaMap : MapBase
    {
        public AntennaMap(string inputData) : base(inputData)
        {

        }

        protected override List<ValuePointCategory<char>> GetValuePointCharCategoriesInternal()
        {
            return [new ValuePointCategory<char>("antenna", GetValuePointChars())];
        }

        private char[] GetValuePointChars()
        {
            var charsToCheckFor = new List<char>();
            var numberChars = Enumerable.Range('0', '9' + 1 - '0').ToList();
            var upperCaseLetters = Enumerable.Range('A', 'Z' + 1 - 'A').ToList();
            var lowerCaseLetters = Enumerable.Range('a', 'z' + 1 - 'a').ToList();
            var concated = numberChars.Concat(upperCaseLetters).Concat(lowerCaseLetters).Select(v => (char)v).ToArray();
            return concated;
        }

        public IDictionary<char, List<Point>> GetAntennaPositionsByFrequency()
        {
            return ValuePointCategories.First().ValuePoints.GroupBy(g => g.Value).ToDictionary(g => g.Key, g => g.Select(v => v.Coordinate).ToList());
        }
    }
}