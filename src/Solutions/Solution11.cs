using aoc_2024.Interfaces;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace aoc_2024.Solutions
{
    public class Solution11 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var stones = inputData.Split(' ').Select(l => new Stone(l)).ToList();
            var stoneCount = stones.Count;
            for (var run = 0; run < 25; run++)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Run {run}..");
                for (var currentIndex = 0; currentIndex < stoneCount; currentIndex++)
                {
                    var mayStone = stones[currentIndex].Blink();
                    if (mayStone != null)
                    {
                        stones.Insert(++currentIndex, mayStone);
                        stoneCount++;
                    }
                }
            }
            return stoneCount.ToString();
        }

        public string RunPartB(string inputData)
        {
            var stones = inputData.Split(' ').Select(l => new Stone(l)).ToList();
            ////var stoneCount = stones.Count;
            ////var dictionary = new Dictionary<long, long>();
            ////for (var run = 0; run < 75; run++)
            ////{
            ////    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Run {run}..");
            ////    for (var currentIndex = 0; currentIndex < stoneCount; currentIndex++)
            ////    {
            ////        var mayStone = stones[currentIndex].BlinkWithCache(dictionary);
            ////        if (mayStone != null)
            ////        {
            ////            stones.Insert(++currentIndex, mayStone);
            ////            stoneCount++;
            ////        }
            ////    }
            ////}
            ////return stoneCount.ToString();
            var processorCount = Environment.ProcessorCount - 2;
            for (var run = 0; run < 75; run++)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Run {run}..");
                List<Stone>[] setsToProcess;
                if (stones.Count < processorCount)
                {
                    setsToProcess = [stones];
                }
                else
                {
                    var chunkSize = stones.Count / processorCount;
                    Console.WriteLine($"Use chunks of size {chunkSize}");
                    //var stopWatch = Stopwatch.StartNew();
                    setsToProcess = stones.Chunk(chunkSize).Select(s => s.ToList()).ToArray();
                    //stopWatch.Stop();
                    //Console.WriteLine($"Took {stopWatch.Elapsed.TotalSeconds} s to chunk (and toList() and toArray())");
                }
                var setsToProcessWithIndex = setsToProcess.Select((q, i) => (Stones: q.ToList(), Index: i)).ToList();
                var setsToConcat = new ConcurrentBag<(List<Stone> Set, int Index)>();
                var calculatedResults = new ConcurrentDictionary<long, long>();
                using var timer = StartLogTimer(calculatedResults);
                var parallelTimeWatch = Stopwatch.StartNew();
                Parallel.ForEach(setsToProcessWithIndex, new ParallelOptions { MaxDegreeOfParallelism = processorCount }, set =>
                {
                    var startTime = DateTime.UtcNow;
                    var newStones = ProcessStones(set.Stones, calculatedResults);
                    var endTime = DateTime.UtcNow;
                    Console.WriteLine($"[{set.Index}] Took {(endTime - startTime).TotalSeconds} s to complete processing..");
                    setsToConcat.Add((newStones, set.Index));
                });
                parallelTimeWatch.Stop();
                Console.WriteLine($"All sets are done. Took {parallelTimeWatch.Elapsed.TotalSeconds} s.");
                var ordered = setsToConcat.OrderBy(s => s.Index);
                stones = [];
                //var sw = Stopwatch.StartNew();
                foreach (var list in ordered)
                {
                    stones.AddRange(list.Set);
                }
                //sw.Stop();
                //Console.WriteLine($"Took {sw.Elapsed.TotalSeconds} s concat the resulting lists..");
            }
            return stones.LongCount().ToString();
        }


        private System.Timers.Timer StartLogTimer(ConcurrentDictionary<long, long> calculatedValues)
        {
            var timer = new System.Timers.Timer();
            var startTime = DateTime.Now;
            timer.Interval = 2000;
            timer.Elapsed += delegate { LogStuff(startTime, calculatedValues); };
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Start();
            return timer;
        }

        private static void LogStuff(DateTime startTime, ConcurrentDictionary<long, long> itemsToLog)
        {
            Debug.WriteLine($"Calculated {itemsToLog.Count} values. [Running for {(DateTime.Now - startTime).TotalSeconds} seconds]");
        }

        //private static string NeverEndingPartB(List<Stone> stones)
        //{
        //    var processorCount = Environment.ProcessorCount;
        //    for (var run = 0; run < 75; run++)
        //    {
        //        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Run {run}..");
        //        List<Stone>[] setsToProcess;
        //        if (stones.Count < processorCount)
        //        {
        //            setsToProcess = [stones];
        //        }
        //        else
        //        {
        //            var chunkSize = stones.Count / processorCount;
        //            Console.WriteLine($"Use chunks of size {chunkSize}");
        //            var stopWatch = Stopwatch.StartNew();
        //            setsToProcess = stones.Chunk(chunkSize).Select(s => s.ToList()).ToArray();
        //            stopWatch.Stop();
        //            Console.WriteLine($"Took {stopWatch.Elapsed.TotalSeconds} s to chunk (and toList() and toArray())");
        //        }
        //        var setsToProcessWithIndex = setsToProcess.Select((q, i) => (Stones: q.ToList(), Index: i)).ToList();
        //        var setsToConcat = new ConcurrentBag<(List<Stone> Set, int Index)>();
        //        var parallelTimeWatch = Stopwatch.StartNew();
        //        Parallel.ForEach(setsToProcessWithIndex, new ParallelOptions { MaxDegreeOfParallelism = processorCount }, set =>
        //        {
        //            var startTime = DateTime.UtcNow;
        //            var newStones = ProcessStones(set.Stones);
        //            var endTime = DateTime.UtcNow;
        //            Console.WriteLine($"[{set.Index}] Took {(endTime - startTime).TotalSeconds} s to complete processing..");
        //            setsToConcat.Add((newStones, set.Index));
        //        });
        //        parallelTimeWatch.Stop();
        //        Console.WriteLine($"All sets are done. Took {parallelTimeWatch.Elapsed.TotalSeconds} s.");
        //        var ordered = setsToConcat.OrderBy(s => s.Index);
        //        stones = [];
        //        var sw = Stopwatch.StartNew();
        //        foreach (var list in ordered)
        //        {
        //            stones.AddRange(list.Set);
        //        }
        //        sw.Stop();
        //        Console.WriteLine($"Took {sw.Elapsed.TotalSeconds} s concat the resulting lists..");
        //    }
        //    return stones.LongCount().ToString();
        //}

        private static List<Stone> ProcessStones(List<Stone> stones, ConcurrentDictionary<long, long> calculatedResults)
        {
            var currentStoneCount = stones.Count;
            for (var currentIndex = 0; currentIndex < currentStoneCount; currentIndex++)
            {
                var mayStone = stones[currentIndex].BlinkWithCache(calculatedResults);
                if (mayStone != null)
                {
                    stones.Insert(++currentIndex, mayStone);
                    currentStoneCount++;
                }
            }
            return stones;
        }
    }

    internal class Stone
    {
        private static readonly int[] EvenNumbers = [2, 4, 6, 8, 0];

        private long CurrentValue { get; set; }

        public Stone(string valueToSet)
        {
            CurrentValue = long.Parse(valueToSet);
        }
        public Stone? Blink()
        {
            if (CurrentValue == 0)
            {
                CurrentValue = 1;
                return null;
            }
            var currentNumberDigitsCount = CurrentValue.Digits_Log10();
            var currentNumberDigitsCountLastValue = currentNumberDigitsCount % 10;
            if (EvenNumbers.Contains(currentNumberDigitsCountLastValue))
            {
                var currentValueString = CurrentValue.ToString();
                var leftSideString = currentValueString[..(currentNumberDigitsCount / 2)];
                CurrentValue = long.Parse(leftSideString);
                var newStone = new Stone(currentValueString[leftSideString.Length..]);
                return newStone;
            }
            CurrentValue *= 2024;
            return null;
        }

        public Stone? BlinkWithCache(ConcurrentDictionary<long, long> calculationResults)
        {
            if (CurrentValue == 0)
            {
                CurrentValue = 1;
                return null;
            }
            if (calculationResults.TryGetValue(CurrentValue, out var resultValue))
            {
                CurrentValue = resultValue;
                return null;
            }
            var currentNumberDigitsCount = CurrentValue.Digits_Log10();
            var currentNumberDigitsCountLastValue = currentNumberDigitsCount % 10;
            if (EvenNumbers.Contains(currentNumberDigitsCountLastValue))
            {
                var currentValueString = CurrentValue.ToString();
                var leftSideString = currentValueString[..(currentNumberDigitsCount / 2)];
                CurrentValue = long.Parse(leftSideString);
                var newStone = new Stone(currentValueString[leftSideString.Length..]);
                return newStone;
            }
            CurrentValue *= 2024;
            return null;
        }
    }

    /// <summary>
    /// See https://stackoverflow.com/a/51099524/8168837
    /// </summary>
    public static class Int64Extensions
    {
        // IF-CHAIN:
        public static int Digits_IfChain(this long n)
        {
            if (n >= 0)
            {
                if (n < 10L) return 1;
                if (n < 100L) return 2;
                if (n < 1000L) return 3;
                if (n < 10000L) return 4;
                if (n < 100000L) return 5;
                if (n < 1000000L) return 6;
                if (n < 10000000L) return 7;
                if (n < 100000000L) return 8;
                if (n < 1000000000L) return 9;
                if (n < 10000000000L) return 10;
                if (n < 100000000000L) return 11;
                if (n < 1000000000000L) return 12;
                if (n < 10000000000000L) return 13;
                if (n < 100000000000000L) return 14;
                if (n < 1000000000000000L) return 15;
                if (n < 10000000000000000L) return 16;
                if (n < 100000000000000000L) return 17;
                if (n < 1000000000000000000L) return 18;
                return 19;
            }
            else
            {
                if (n > -10L) return 2;
                if (n > -100L) return 3;
                if (n > -1000L) return 4;
                if (n > -10000L) return 5;
                if (n > -100000L) return 6;
                if (n > -1000000L) return 7;
                if (n > -10000000L) return 8;
                if (n > -100000000L) return 9;
                if (n > -1000000000L) return 10;
                if (n > -10000000000L) return 11;
                if (n > -100000000000L) return 12;
                if (n > -1000000000000L) return 13;
                if (n > -10000000000000L) return 14;
                if (n > -100000000000000L) return 15;
                if (n > -1000000000000000L) return 16;
                if (n > -10000000000000000L) return 17;
                if (n > -100000000000000000L) return 18;
                if (n > -1000000000000000000L) return 19;
                return 20;
            }
        }

        // USING LOG10:
        public static int Digits_Log10(this long n) =>
            n == 0L ? 1 : (n > 0L ? 1 : 2) + (int)Math.Log10(Math.Abs((double)n));

        // WHILE LOOP:
        public static int Digits_While(this long n)
        {
            int digits = n < 0 ? 2 : 1;
            while ((n /= 10L) != 0L) ++digits;
            return digits;
        }

        // STRING CONVERSION:
        public static int Digits_String(this long n) =>
            n.ToString().Length;
    }
}