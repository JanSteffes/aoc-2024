using aoc_2024.Interfaces;

namespace aoc_2024.Solutions
{
    public class Solution09 : ISolution
    {
        private const long FreeSpaceValue = -1;

        public string RunPartA(string inputData)
        {
            var fileDigits = inputData.Trim().Chunk(2).Select((x, index) => new SpaceEntry(index, int.Parse(x.First().ToString()), int.Parse(x.Last().ToString()))).ToList();
            var map = CreateSingleEntryMap(fileDigits);
            ProcessMapA(map);
            var checkSum = map.Select((entry, index) => entry.Value > FreeSpaceValue ? index * entry.Value : 0).Sum();
            // 6211348208140 
            return checkSum.ToString();
        }

        public string RunPartB(string inputData)
        {
            var fileDigits = inputData.Trim().Chunk(2).Select((x, index) => new SpaceEntry(index, int.Parse(x.First().ToString()), int.Parse(x.Last().ToString()))).ToList();
            var map = CreateBlockEntryMap(fileDigits);
            ProcessMapB(map);
            var checkSum = map.Select((entry, index) => entry.Value > FreeSpaceValue ? index * entry.Value : 0).Sum();
            // ? 
            return checkSum.ToString();
        }

        private static void ProcessMapA(List<MapEntry> map)
        {
            var firstFreeSpacePos = 0;
            var lastNumberSpacePos = map.Count - 1;
            while (map.FindIndex(firstFreeSpacePos, e => e.Value == FreeSpaceValue) is { } currentFirstFreeSpacePos
                && map.FindLastIndex(lastNumberSpacePos, e => e.Value > FreeSpaceValue) is { } currentLastValuePos
                && currentFirstFreeSpacePos < currentLastValuePos)
            {
                (map[currentLastValuePos], map[currentFirstFreeSpacePos]) = (map[currentFirstFreeSpacePos], map[currentLastValuePos]);
                firstFreeSpacePos = currentFirstFreeSpacePos;
                lastNumberSpacePos = currentLastValuePos;
            }
        }
        private static List<MapEntry> CreateSingleEntryMap(List<SpaceEntry> spaceEntries)
        {
            var mapEntries = new List<MapEntry>();
            var last = spaceEntries.Last();
            foreach (var spaceEntry in spaceEntries)
            {
                var valueEntries = Enumerable.Repeat(new MapEntry(spaceEntry.Value), spaceEntry.Files);
                mapEntries.AddRange(valueEntries);
                if (spaceEntry != last)
                {
                    var freeSpaceEntries = Enumerable.Repeat(new MapEntry(FreeSpaceValue), spaceEntry.FreeSpace);
                    mapEntries.AddRange(freeSpaceEntries);
                }
            }
            return mapEntries;
        }

        private static void ProcessMapB(List<MapBlock> map)
        {
            var currentMaxId = map.Max(m => m.Value);
            var minId = map.Min(m => m.Value);
            var lastLargestValuePos = map.Count - 1;
            // prepare dict of empty spaces with their indexes
            var emptySpaceIndexesWithRange = map.Where(m => m.Value == FreeSpaceValue)
                .GroupBy(g => g.Range)
                .ToDictionary(
                    k => k.Key,
                    k => k.Select(q => (EmptySpaceBlock: q, Index: map.IndexOf(q))).ToList());

            while (currentMaxId > minId)
            {
                lastLargestValuePos = map.FindLastIndex(lastLargestValuePos, m => m.Value == currentMaxId);
                var currentMaxIdBlock = map[lastLargestValuePos];
                var spaceNeeded = currentMaxIdBlock.Range;
                var matchingSpaces = emptySpaceIndexesWithRange.Where(e => e.Key >= spaceNeeded).ToList();
                if (matchingSpaces.Count == 0)
                {
                    currentMaxId++;
                    continue;
                }

                var firstMatchingSpaceWithRangeGroup = matchingSpaces.MinBy(e => e.Value.MinBy(q => q.Index));
                var firstMatchingEmptySpace = firstMatchingSpaceWithRangeGroup.Value.MinBy(q => q.Index)!;

                firstMatchingSpaceWithRangeGroup.Value.Remove(firstMatchingEmptySpace);

                (map[lastLargestValuePos], map[firstMatchingEmptySpace.Index]) = (map[firstMatchingEmptySpace.Index], map[lastLargestValuePos]);
                var newEmptySpaceRange = firstMatchingEmptySpace.EmptySpaceBlock.Range - spaceNeeded;
                if (newEmptySpaceRange > 0)
                {
                    // TODO prepare new empty space behind firstMatchingEmptySpaceIndex and add in dict and increase index of all spaces with > index by 1
                    // TODO set empty space at lastLargetValuePos to range of replaces block
                }
                currentMaxId--;
            }
        }

        private static List<MapBlock> CreateBlockEntryMap(List<SpaceEntry> spaceEntries)
        {
            var mapEntries = new List<MapBlock>();
            var last = spaceEntries.Last();
            foreach (var spaceEntry in spaceEntries)
            {
                mapEntries.Add(new MapBlock(spaceEntry.Value, spaceEntry.Files));
                if (spaceEntry != last && spaceEntry.FreeSpace > 0)
                {
                    mapEntries.Add(new MapBlock(FreeSpaceValue, spaceEntry.FreeSpace));
                }
            }
            return mapEntries;
        }


    }

    class MapBlock : MapEntry
    {
        public int Range { get; set; }
        public MapBlock(long value, int range) : base(value)
        {
            Range = range;
        }
    }

    class MapEntry
    {
        public long Value { get; private set; }

        public MapEntry(long value)
        {
            Value = value;
        }
    }

    class SpaceEntry
    {
        public int Value { get; private set; }

        public int Files { get; private set; }

        public int FreeSpace { get; private set; }

        public SpaceEntry(int value, int files, int freeSpace)
        {
            Value = value;
            Files = files;
            FreeSpace = freeSpace;
        }
    }
}