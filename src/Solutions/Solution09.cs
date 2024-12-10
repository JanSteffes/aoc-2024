using aoc_2024.Interfaces;
using System.Diagnostics;

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
            WriteMapToDebug(map);
            ProcessMapB(map);
            var index = 0;
            var checkSum = 0L;
            foreach (var entry in map)
            {
                if (entry.Value == FreeSpaceValue)
                {
                    index += entry.Range;
                    continue;
                }
                checkSum += entry.CalculateValue(index);
                if (checkSum > 6239783302560)
                {
                    Debug.WriteLine($"Failed at index {index}, for entry at {map.IndexOf(entry)}");
                    WriteMapToDebug(map);
                    break;
                }
                index += entry.Range;
            }
            // 8358671598691 too high
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

            //WriteMapToDebug(map);
            var logId = currentMaxId;
            var nextLogId = currentMaxId - 20;
            while (currentMaxId > minId)
            {
                if (logId-- == nextLogId)
                {
                    //Debug.WriteLine($"Processing {logId}..");
                    nextLogId -= 20;
                }
                try
                {
                    lastLargestValuePos = map.FindLastIndex(lastLargestValuePos, m => m.Value == currentMaxId);
                    if (lastLargestValuePos < 0)
                    {
                        //Debug.WriteLine($"# not moving {currentMaxId} because not found anymore");
                        currentMaxId--;
                        continue;
                    }
                    var currentMaxIdBlock = map[lastLargestValuePos];
                    var spaceNeeded = currentMaxIdBlock.Range;
                    var matchingSpaces = emptySpaceIndexesWithRange.Where(e => e.Key >= spaceNeeded).ToList();
                    if (matchingSpaces.Count == 0)
                    {
                        //Debug.WriteLine($"# not moving {currentMaxId} because no matching space found");
                        currentMaxId--;
                        continue;
                    }


                    var firstMatchingSpaceWithRangeGroup = matchingSpaces.MinBy(matchingSpace => matchingSpace.Value.MinBy(q => q.Index).Index);
                    //{
                    //    try
                    //    {
                    //        return matchingSpace.Value.MinBy(q => q.Index).Index;
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        Debug.WriteLine(e);
                    //    }
                    //    return int.MaxValue;
                    //});
                    var firstMatchingEmptySpace = firstMatchingSpaceWithRangeGroup.Value.MinBy(q => q.Index)!;
                    if (firstMatchingSpaceWithRangeGroup.Value.Count == 1)
                    {
                        emptySpaceIndexesWithRange.Remove(firstMatchingSpaceWithRangeGroup.Key);
                    }
                    else
                    {
                        firstMatchingSpaceWithRangeGroup.Value.Remove(firstMatchingEmptySpace);
                    }

                    //Debug.WriteLine($"# swapping {currentMaxId} at {lastLargestValuePos} with {firstMatchingEmptySpace.Index}");

                    (map[lastLargestValuePos], map[firstMatchingEmptySpace.Index]) = (map[firstMatchingEmptySpace.Index], map[lastLargestValuePos]);

                    // calc and check if already too much
                    var currentSum = map.Take(firstMatchingEmptySpace.Index + 1).Select((e, index) => e.CalculateValue(index)).Sum();
                    if (currentSum > 6239783302560)
                    {
                        WriteMapToDebug(map);
                        throw new ArithmeticException($"Result already too hight!");
                    }

                    var newEmptySpaceRange = firstMatchingEmptySpace.EmptySpaceBlock.Range - spaceNeeded;
                    if (newEmptySpaceRange > 0)
                    {
                        // TODO set empty space at lastLargetValuePos to range of replaces block
                        map[lastLargestValuePos].Range = spaceNeeded;
                        // TODO prepare new empty space behind firstMatchingEmptySpaceIndex and add in dict and increase index of all spaces with > index by 1
                        var newEmptySpaceIndex = firstMatchingEmptySpace.Index + 1;
                        var newEmptySpace = new MapBlock(FreeSpaceValue, newEmptySpaceRange);
                        if (emptySpaceIndexesWithRange.TryGetValue(newEmptySpace.Range, out var emptySpaceList))
                        {
                            emptySpaceList.Add((newEmptySpace, newEmptySpaceIndex));
                        }
                        else
                        {
                            emptySpaceIndexesWithRange.Add(newEmptySpace.Range, new List<(MapBlock EmptySpaceBlock, int Index)> { (newEmptySpace, newEmptySpaceIndex) });
                        }
                        map.Insert(newEmptySpaceIndex, newEmptySpace);
                        foreach (var group in emptySpaceIndexesWithRange)
                        {
                            for (var index = 0; index < group.Value.Count; index++)
                            {
                                if (group.Value[index].Index > newEmptySpaceIndex)
                                {
                                    group.Value[index] = (new MapBlock(FreeSpaceValue, group.Value[index].EmptySpaceBlock.Range), group.Value[index].Index + 1);
                                }
                            }
                        }
                    }
                    currentMaxId--;
                    // TODO: outcommend when running for input
                    //WriteMapToDebug(map);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"{e.GetType().Name}: {e.Message}");
                }
            }
        }

        private static void WriteMapToDebug(List<MapBlock> map)
        {
            var mapString = string.Empty;
            foreach (var block in map)
            {
                mapString += "|" + string.Join(string.Empty, Enumerable.Repeat(block.Value.ToString(), block.Range));
            }
            mapString = mapString.Replace("-1", ".");
            Debug.WriteLine(mapString);
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
                sum += (index + current) * Value;
            }
            return sum;
        }
    }

    public class MapEntry
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