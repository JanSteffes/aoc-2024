using aoc_2024.Interfaces;
using aoc_2024.Solutions.Helper;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution15 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var lines = ParseUtils.ParseIntoLines(inputData);
            var mapLines = new List<string>();
            var moveLines = new List<string>();
            foreach (var line in lines)
            {
                if (line.Contains('#'))
                {
                    mapLines.Add(line);
                }
                else
                {
                    moveLines.Add(line);
                }
            }
            var map = new WarehouseMap(string.Join(Environment.NewLine, mapLines));
            map.PrintMap();

            var moveSets = ParseDirections(string.Join(string.Empty, moveLines));
            foreach (var move in moveSets)
            {
                map.MoveGuard(move);
            }

            return string.Empty;
        }

        public string RunPartB(string inputData)
        {
            throw new NotImplementedException();
        }


        private List<Direction> ParseDirections(string moveset)
        {
            return moveset.Select(c => (Direction)Enum.ToObject(typeof(Direction), c)).ToList();
        }


    }


    class WarehouseMap : MapBase
    {
        private readonly char[] obstacleChars = ['#'];

        public WarehouseMap(string inputData) : base(inputData)
        {
        }

        internal void MoveGuard(Direction move)
        {
            throw new NotImplementedException();
        }
    }
}