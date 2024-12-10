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
            var checkSum = map.Select((entry, index) => entry.FileId > FreeSpaceValue ? index * entry.FileId : 0).Sum();
            // 6211348208140 
            return checkSum.ToString();
        }

        public string RunPartB(string inputData)
        {
            var fileDigits = inputData.Trim().Chunk(2).Select((x, index) => new SpaceEntry(index, int.Parse(x.First().ToString()), int.Parse(x.Last().ToString()))).ToList();
            var map = CreateBlockEntryMap(fileDigits);
            ProcessMapB(map);
            var sum = 0L;
            var positionIndex = 0;
            for (var index = 0; index < map.Count; index++)
            {
                var currentEntry = map[index];
                if (currentEntry.FileId == FreeSpaceValue)
                {
                    positionIndex += currentEntry.Range;
                }
                else
                {
                    sum += currentEntry.CalculateValue(positionIndex);
                    positionIndex += currentEntry.Range;
                }
            }
            // 6239783302560
            return sum.ToString();
        }

        private void ProcessMapB(List<MapBlock> map)
        {
            var currentMaxFileId = map.Max(a => a.FileId) + 1;
            var currentMaxIndex = map.Count - 1;
            while (currentMaxFileId-- > 0)
            {
                var currentMaxBlockIndex = map.FindLastIndex(currentMaxIndex, e => e.FileId == currentMaxFileId);
                if (currentMaxBlockIndex == -1)
                {
                    continue;
                }
                var currentMaxBlock = map[currentMaxBlockIndex];
                var rangeToSearchFor = currentMaxBlock.Range;
                var firstMatchingFreeSpaceIndex = map.FindIndex(m => m.FileId == FreeSpaceValue && m.Range >= rangeToSearchFor);
                if (firstMatchingFreeSpaceIndex == -1)
                {
                    continue;
                }
                if (firstMatchingFreeSpaceIndex > currentMaxBlockIndex)
                {
                    continue;
                }
                var freeSpaceBlock = map[firstMatchingFreeSpaceIndex];
                if (rangeToSearchFor == freeSpaceBlock.Range)
                {
                    (map[currentMaxBlockIndex], map[firstMatchingFreeSpaceIndex]) = (map[firstMatchingFreeSpaceIndex], map[currentMaxBlockIndex]);
                }
                else
                {
                    // swap partly
                    // replace free block with currentMaxBlock
                    map[firstMatchingFreeSpaceIndex] = currentMaxBlock;
                    // replace map[currentMaxBlockIndex] with empty block with same range
                    var newFreeBlock = new MapBlock(FreeSpaceValue, rangeToSearchFor);
                    map[currentMaxBlockIndex] = newFreeBlock;
                    // and add free block behind map[firstMatchingFreeSpaceIndex] with remaing range
                    var remainingFreeSpaceBlock = new MapBlock(FreeSpaceValue, freeSpaceBlock.Range - rangeToSearchFor);
                    map.Insert(firstMatchingFreeSpaceIndex + 1, remainingFreeSpaceBlock);
                }

            }
        }

        private static void ProcessMapA(List<MapEntry> map)
        {
            var firstFreeSpacePos = 0;
            var lastNumberSpacePos = map.Count - 1;
            while (map.FindIndex(firstFreeSpacePos, e => e.FileId == FreeSpaceValue) is { } currentFirstFreeSpacePos
                && map.FindLastIndex(lastNumberSpacePos, e => e.FileId > FreeSpaceValue) is { } currentLastValuePos
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
                var valueEntries = Enumerable.Repeat(new MapEntry(spaceEntry.FileId), spaceEntry.Files);
                mapEntries.AddRange(valueEntries);
                if (spaceEntry != last)
                {
                    var freeSpaceEntries = Enumerable.Repeat(new MapEntry(FreeSpaceValue), spaceEntry.FreeSpace);
                    mapEntries.AddRange(freeSpaceEntries);
                }
            }
            return mapEntries;
        }

        private static List<MapBlock> CreateBlockEntryMap(List<SpaceEntry> spaceEntries)
        {
            var mapEntries = new List<MapBlock>();
            var last = spaceEntries.Last();
            foreach (var spaceEntry in spaceEntries)
            {
                mapEntries.Add(new MapBlock(spaceEntry.FileId, spaceEntry.Files));
                if (spaceEntry != last && spaceEntry.FreeSpace > 0)
                {
                    mapEntries.Add(new MapBlock(FreeSpaceValue, spaceEntry.FreeSpace));
                }
            }
            return mapEntries;
        }

    }

    public class MapBlock : MapEntry
    {
        public int Range { get; set; }
        public MapBlock(long value, int range) : base(value)
        {
            Range = range;
        }

        public long CalculateValue(int index)
        {
            var sum = 0L;
            for (var current = 0; current < Range; current++)
            {
                sum += (index + current) * FileId;
            }
            return sum;
        }
    }

    public class MapEntry
    {
        public long FileId { get; private set; }

        public MapEntry(long value)
        {
            FileId = value;
        }
    }

    class SpaceEntry
    {
        public int FileId { get; private set; }

        public int Files { get; private set; }

        public int FreeSpace { get; private set; }

        public SpaceEntry(int value, int files, int freeSpace)
        {
            FileId = value;
            Files = files;
            FreeSpace = freeSpace;
        }
    }
}