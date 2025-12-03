using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace aoc_2024.Solutions
{
    public class Solution19 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var lines = ParseUtils.ParseIntoLines(inputData);
            var patterns = lines.First().Split(",").Select(s => s.Trim()).ToList();
            var designs = lines.Skip(1).Select(s => new Design(s.Trim())).ToList();

            var possible = new ConcurrentBag<string>();
            var cache = new ConcurrentDictionary<string, bool>();

            var possibleDesignsCount = 0;
            var count = 0;
            var designsCount = designs.Count;
            foreach (var desingToMatch in designs)
            {

                Debug.WriteLine($"Processing design {++count} of {designsCount}..");
                if (desingToMatch.NewApproach(patterns))
                {
                    possible.Add(desingToMatch.Value);
                }
            }
            possibleDesignsCount = possible.Count();
            return possibleDesignsCount.ToString();
        }


        public string RunPartB(string inputData)
        {
            var lines = ParseUtils.ParseIntoLines(inputData);
            var patterns = lines.First().Split(",").Select(s => s.Trim()).ToList();
            var designs = lines.Skip(1).Select(s => new Design(s.Trim())).ToList();

            var possible = new ConcurrentDictionary<string, int>();
            var cache = new ConcurrentDictionary<string, bool>();

            var possibleDesignsCount = 0;
            var count = 0;
            var designsCount = designs.Count;
            foreach (var desingToMatch in designs)
            {

                Debug.WriteLine($"Processing design {++count} of {designsCount}..");
                if (desingToMatch.NewApproachWays(patterns) > 0)
                {
                    possible.TryAdd(desingToMatch.Value, desingToMatch.GetPossiblePatterns());
                }
            }
            possibleDesignsCount = possible.Sum(q => q.Value);
            return possibleDesignsCount.ToString();
        }
    }


    class Pattern
    {
        public string Stripes { get; set; }

        public Pattern(string stripePattern)
        {
            Stripes = stripePattern;
        }

        public override string ToString()
        {
            return Stripes;
        }
    }

    class Design
    {
        static HashSet<string> PossiblePatterns = new HashSet<string>();

        static HashSet<string> ImpossiblePatterns = new HashSet<string>();

        static Dictionary<string, bool> DesignCache = new Dictionary<string, bool>();

        public string Value { get; set; }

        private int PossibleWays = 0;

        public Design(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        internal bool IsPossibleRecursiveNew(List<string> patterns)
        {
            return IsPossibleRecursiveRecursiveStep(patterns, Value);
        }

        internal static bool IsPossibleRecursiveRecursiveStep(List<string> patterns, string designToMatch)
        {
            Debug.WriteLine($"Testing {designToMatch}...");
            if (string.IsNullOrEmpty(designToMatch) || PossiblePatterns.Contains(designToMatch))
            {
                return true;
            }
            if (ImpossiblePatterns.Contains(designToMatch))
            {
                return false;
            }
            var matchingPatterns = patterns.Where(s => designToMatch.StartsWith(s, StringComparison.Ordinal)).ToList();
            foreach (var pattern in matchingPatterns)
            {
                var leftDesignToMatch = designToMatch.Substring(pattern.Length);
                var patternsContainedInDesign = patterns.Where(p => leftDesignToMatch.Contains(p, StringComparison.Ordinal)).ToList();
                var canBeMatched = leftDesignToMatch.Distinct().All(d => patternsContainedInDesign.Any(p => leftDesignToMatch.Contains(d, StringComparison.Ordinal)));
                if (!canBeMatched)
                {
                    ImpossiblePatterns.Add(designToMatch);
                    return false;
                }
                if (IsPossibleRecursiveRecursiveStep(patternsContainedInDesign, leftDesignToMatch))
                {
                    PossiblePatterns.Add(designToMatch);
                    return true;
                }
            }
            return false;
        }

        internal bool IsPossibleWithPatterns(HashSet<string> patterns, ConcurrentDictionary<string, bool> cache)
        {
            return IsPossibleWithPatternsRecursive(patterns, Value, cache);
        }

        internal bool IsPossibleWithPatternsRecursive(HashSet<string> patterns, string designToMatch, ConcurrentDictionary<string, bool> cache)
        {
            if (cache.TryGetValue(designToMatch, out var result))
            {
                return result;
            }
            if (patterns.Contains(designToMatch))
            {
                cache.TryAdd(designToMatch, true);
                return true;
            }
            foreach (var pattern in patterns)
            {
                if (designToMatch.StartsWith(pattern))
                {
                    var possible = IsPossibleWithPatternsRecursive(patterns, designToMatch.Substring(pattern.Length), cache);
                    if (possible)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal bool IsPossibleWithPatternsComplexAlsoOld(Dictionary<char, Dictionary<int, List<string>>> patterns, ConcurrentDictionary<string, bool> cache)
        {
            return IsPossibleWithPatternsComplexAlsoOld(patterns, Value, cache);

        }
        internal static bool IsPossibleWithPatternsComplexOld(Dictionary<char, Dictionary<int, List<string>>> patterns, string designToMatch, ConcurrentDictionary<string, bool> cache)
        {
            if (cache.TryGetValue(designToMatch, out bool value))
            {
                return value;
            }
            var firstChar = designToMatch[0];
            if (!patterns.TryGetValue(firstChar, out var possiblePatternsByStartKey))
            {
                return false;
            }
            var lengthToMatch = designToMatch.Length;
            var possiblePatternsByCharAndLength = possiblePatternsByStartKey.Where(p => p.Key <= lengthToMatch).SelectMany(q => q.Value).ToList();
            if (possiblePatternsByCharAndLength.Any(q => q.Contains(designToMatch)))
            {
                cache.TryAdd(designToMatch, true);
                return true;
            }
            var reducedPossiblePatternsByCharAndLength = possiblePatternsByStartKey.Where(p => p.Key <= lengthToMatch - 1).SelectMany(q => q.Value).ToList();
            foreach (var pattern in reducedPossiblePatternsByCharAndLength)
            {
                if (designToMatch.StartsWith(pattern))
                {
                    var remainingDesign = designToMatch[pattern.Length..];
                    var possible = IsPossibleWithPatternsComplexAlsoOld(patterns, remainingDesign, cache);
                    if (possible)
                    {
                        cache.TryAdd(designToMatch, possible);
                        return true;
                    }
                }
            }
            return false;
        }


        internal static bool IsPossibleWithPatternsComplexAlsoOld(Dictionary<char, Dictionary<int, List<string>>> patterns, string designToMatch, ConcurrentDictionary<string, bool> cache)
        {
            if (cache.TryGetValue(designToMatch, out bool value))
            {
                return value;
            }
            var firstChar = designToMatch[0];
            if (!patterns.TryGetValue(firstChar, out var possiblePatternsByStartKey))
            {
                return false;
            }
            var lengthToMatch = designToMatch.Length;
            var possiblePatternsByCharAndLength = possiblePatternsByStartKey.Where(p => p.Key <= lengthToMatch).SelectMany(q => q.Value).ToList();
            if (possiblePatternsByCharAndLength.Any(q => q.Contains(designToMatch, StringComparison.Ordinal)))
            {
                cache.TryAdd(designToMatch, true);
                return true;
            }
            var patternsNeeded = patterns.SelectMany(p => p.Value.SelectMany(q => q.Value).Where(p => designToMatch.Contains(p, StringComparison.Ordinal))).ToList();
            var smallPatternDict = patternsNeeded.GroupBy(p => p[0]).ToDictionary(d => d.Key, d => d.GroupBy(q => q.Length).ToDictionary(v => v.Key, v => v.ToList()));
            foreach (var pattern in patternsNeeded)
            {
                if (designToMatch.StartsWith(pattern, StringComparison.Ordinal))
                {
                    var remainingDesign = designToMatch[pattern.Length..];
                    var possible = IsPossibleWithPatternsComplexAlsoOld(smallPatternDict, remainingDesign, cache);
                    if (possible)
                    {
                        cache.TryAdd(designToMatch, possible);
                        return true;
                    }
                }
            }
            return false;
        }

        internal bool RegexApproachNotWorking(List<string> patterns)
        {
            var regexString = "(" + string.Join("|", patterns.OrderByDescending(p => p.Length)) + ")+";
            var regex = new Regex(regexString);
            var matches = regex.Match(Value);
            var valid = matches.Groups.Values.Any(v => v.Value == Value);
            return valid;
        }

        internal bool NewApproach(List<string> patterns)
        {
            var patternsBySize = patterns.GroupBy(d => d.Length, d => d).ToDictionary(d => d.Key, d => d.ToList());
            SimplifyPatterns(patternsBySize);
            var patternsByLetter = patternsBySize.SelectMany(p => p.Value).GroupBy(p => p.First()).ToDictionary(p => p.Key, p => p.ToList());
            return IsComposeable(patternsBySize, patternsByLetter, Value);
        }

        internal int NewApproachWays(List<string> patterns)
        {
            var patternsBySize = patterns.GroupBy(d => d.Length, d => d).ToDictionary(d => d.Key, d => d.ToList());
            SimplifyPatterns(patternsBySize);
            var patternsByLetter = patternsBySize.SelectMany(p => p.Value).GroupBy(p => p.First()).ToDictionary(p => p.Key, p => p.ToList());
            return IsComposeableWays(patternsBySize, patternsByLetter, Value);
        }

        private int IsComposeableWays(Dictionary<int, List<string>> patternsBySize, Dictionary<char, List<string>> patternsByLetter, string designToMatch)
        {
            var composeableWays = 0;
            if (string.IsNullOrEmpty(designToMatch))
            {
                return 1;
            }
            foreach (var pattern in patternsByLetter.TryGetValue(designToMatch[0], out var matchingPatternsByCharList) ? matchingPatternsByCharList : [])
            {
                if (!designToMatch.StartsWith(pattern, StringComparison.Ordinal))
                {
                    continue;
                }
                var remaining = designToMatch[pattern.Length..];
                if (string.IsNullOrEmpty(remaining))
                {
                    if (!DesignCache.TryAdd(designToMatch, true))
                    {
                        DesignCache[designToMatch] = true;
                    }
                    composeableWays++;
                }
                composeableWays += IsComposeableWays(patternsBySize, patternsByLetter, remaining);
            }
            if (composeableWays > 0)
            {
                return composeableWays;
            }
            if (!DesignCache.TryAdd(designToMatch, false))
            {
                DesignCache[designToMatch] = false;
            }
            return 0;
        }

        private bool IsComposeable(Dictionary<int, List<string>> patternsBySize, Dictionary<char, List<string>> patternsByLetter, string designToMatch)
        {
            if (DesignCache.TryGetValue(designToMatch, out var value))
            {
                return value;
            }
            foreach (var pattern in patternsByLetter.TryGetValue(designToMatch[0], out var matchingPatternsByCharList) ? matchingPatternsByCharList : [])
            {
                if (!designToMatch.StartsWith(pattern, StringComparison.Ordinal))
                {
                    continue;
                }
                var remaining = designToMatch[pattern.Length..];
                if (string.IsNullOrEmpty(remaining))
                {
                    if (!DesignCache.TryAdd(designToMatch, true))
                    {
                        DesignCache[designToMatch] = true;
                    }
                    return true;
                }
                if (IsComposeable(patternsBySize, patternsByLetter, remaining))
                {
                    return true;
                }
            }
            if (!DesignCache.TryAdd(designToMatch, false))
            {
                DesignCache[designToMatch] = false;
            }
            return false;
        }

        private static void SimplifyPatterns(Dictionary<int, List<string>> patternsBySize)
        {
            var maxSize = patternsBySize.Keys.Count;

            for (var size = 2; size <= maxSize; size++)
            {
                var reducedPatternsForSize = GetSimplifiedPatternsBySize(size, patternsBySize);
                patternsBySize[size] = reducedPatternsForSize;
            }

        }

        private static List<string> GetSimplifiedPatternsBySize(int size, Dictionary<int, List<string>> patternsBySize)
        {
            var newList = new List<string>();

            foreach (var pattern in patternsBySize[size])
            {
                if (!PatternIsRedundant(pattern, 1, patternsBySize))
                {
                    newList.Add(pattern);
                }
            }
            return newList;
        }

        private static bool PatternIsRedundant(string patternToSimplify, int reductor, Dictionary<int, List<string>> patternsBySize)
        {
            var maxSize = patternToSimplify.Length - reductor;
            for (var size = maxSize; size > 0; size--)
            {
                foreach (var pattern in patternsBySize[size])
                {
                    if (!patternToSimplify.StartsWith(pattern, StringComparison.Ordinal))
                    {
                        continue;
                    }
                    var remaining = patternToSimplify.Replace(pattern, string.Empty);

                    if (string.IsNullOrEmpty(remaining) || PatternIsRedundant(remaining, 0, patternsBySize))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal int GetPossiblePatterns()
        {
            return PossibleWays;
        }
    }
}