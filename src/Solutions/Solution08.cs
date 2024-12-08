using aoc_2024.Interfaces;
using aoc_2024.Solutions.Helper;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;

namespace aoc_2024.Solutions
{
    public class Solution08 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var map = new AntennaMap(inputData);
            var antennaPositionsByFrequency = map.GetAntennaPositionsByFrequency();
            var antennaPositionsCount = antennaPositionsByFrequency.Sum(a => a.Value.Count);
            var antinodes = new Dictionary<char, List<Point>>();
            var filderedAntinodes = new Dictionary<char, List<Point>>();
            foreach (var entry in antennaPositionsByFrequency)
            {
                filderedAntinodes.Add(entry.Key, []);
            }
            foreach (var antennaPositionGroup in antennaPositionsByFrequency)
            {
                var currentAntiNodes = ProcessAntennaGroup(map, antennaPositionsByFrequency, filderedAntinodes, antennaPositionGroup);
                antinodes.Add(antennaPositionGroup.Key, currentAntiNodes);
            }
            foreach (var item in antinodes)
            {
                var orderedItems = item.Value.OrderBy(v => v.X).ThenBy(v => v.Y).ToList();
                item.Value.Clear();
                item.Value.AddRange(orderedItems);
            }
            var distinctAntinodes = antinodes.SelectMany(a => a.Value).Distinct().ToList();
            // 426
            return distinctAntinodes.Count.ToString();
        }


        public string RunPartB(string inputData)
        {
            // 1359 should be correct
            throw new NotImplementedException();
        }

        private static List<Point> ProcessAntennaGroup(AntennaMap map, IDictionary<char, List<Point>> antennaPositionsByFrequency, Dictionary<char, List<Point>> filderedAntinodes, KeyValuePair<char, List<Point>> antennaPositionGroup)
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
                        if (currentAntiNodes.All(ap => !ap.Equals(antiNodePosition)))
                        {
                            // why do i not have to check for this?!
                            //if (otherAntennaPositionGroups.All(at => at.Value.All(v => !v.Equals(antiNodePosition))))
                            //{
                            currentAntiNodes.Add(antiNodePosition);
                            //}
                            //else
                            //{
                            //    var antennasOccupyingSpace = otherAntennaPositionGroups.Where(at => at.Value.Any(v => v.Equals(antiNodePosition))).ToList();
                            //    Debug.WriteLine($"[{currentChar}] Filtered {antiNodePosition} because antenna(s) of {string.Join(",", antennasOccupyingSpace.Select(s => s.Key))} already occupy it!");
                            //    filderedAntinodes[currentChar].Add(antiNodePosition);
                            //}
                        }
                        else
                        {
                            Debug.WriteLine($"[{currentChar}] Filtered {antiNodePosition} because alread contained in current antiNodePositions!");
                            filderedAntinodes[currentChar].Add(antiNodePosition);
                        }
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

        protected override ReadOnlyCollection<char> GetValuePointChars()
        {
            var charsToCheckFor = new List<char>();
            var numberChars = Enumerable.Range('0', '9' + 1 - '0').ToList();
            var upperCaseLetters = Enumerable.Range('A', 'Z' + 1 - 'A').ToList();
            var lowerCaseLetters = Enumerable.Range('a', 'z' + 1 - 'a').ToList();
            var concated = numberChars.Concat(upperCaseLetters).Concat(lowerCaseLetters).Select(v => (char)v).ToList();
            return new ReadOnlyCollection<char>(concated);
        }

        public IDictionary<char, List<Point>> GetAntennaPositionsByFrequency()
        {
            return ValuePoints.GroupBy(g => g.Value).ToDictionary(g => g.Key, g => g.Select(v => v.Coordinate).ToList());
        }
    }
}