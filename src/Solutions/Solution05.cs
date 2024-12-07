using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;
using System.Collections.Concurrent;

namespace aoc_2024.Solutions
{
    public class Solution05 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var lines = ParseUtils.ParseIntoLines(inputData);
            var ruleLines = lines.Where(l => l.Contains("|")).ToList();
            var rules = ruleLines.Select(Rule.CreateFromLine).ToList();

            var valueLines = lines.Except(ruleLines).ToList();
            var valueSets = valueLines.Select(s => s.Split(',').Select(int.Parse).ToList()).ToList();

            var validSets = valueSets.Where(set => rules.Where(rule => rule.AppliesToSet(set)).All(rule => rule.IsValidSet(set))).ToList();

            var middlePageSum = validSets.Sum(GetMiddlePageNumber);
            return middlePageSum.ToString();
        }

        public string RunPartB(string inputData)
        {
            var lines = ParseUtils.ParseIntoLines(inputData);
            var ruleLines = lines.Where(l => l.Contains("|")).ToList();
            var rules = ruleLines.Select(Rule.CreateFromLine).ToList();

            var valueLines = lines.Except(ruleLines).ToList();
            var valueSets = valueLines.Select(s => s.Split(',').Select(int.Parse).ToList()).ToList();

            var setsAndRules = valueSets.Select(valueSet => (ValueSet: valueSet, ApplyingRules: rules.Where(rule => rule.AppliesToSet(valueSet)).ToList())).ToList();
            var invalidSetsAndRules = setsAndRules.Where(entry => entry.ApplyingRules.Any(rule => !rule.IsValidSet(entry.ValueSet))).ToList();

            var correctedSets = new ConcurrentBag<List<int>>();
            Parallel.ForEach(invalidSetsAndRules, entry =>
            {
                correctedSets.Add(GetCorrectedSet(entry.ValueSet, entry.ApplyingRules));
            });
            var middlePageSum = correctedSets.Sum(GetMiddlePageNumber);
            return middlePageSum.ToString();
        }

        private static List<int> GetCorrectedSet(List<int> valueSet, List<Rule> rules)
        {
            var latestFixedSet = valueSet.ToList();
            while (rules.Any(r => !r.IsValidSet(latestFixedSet)))
            {
                foreach (var rule in rules)
                {
                    if (!rule.IsValidSet(latestFixedSet))
                    {
                        latestFixedSet = rule.FixSet(latestFixedSet);
                    }
                }
            }
            return latestFixedSet;
        }

        private int GetMiddlePageNumber(List<int> list)
        {
            var length = list.Count;
            var middle = length / 2;
            return list[middle];
        }
    }

    internal class Rule
    {
        private int PrecedingNumber { get; set; }

        private int SucceeedingNumber { get; set; }

        public static Rule CreateFromLine(string line)
        {
            var newRule = new Rule();
            var splitted = line.Split('|');
            newRule.PrecedingNumber = int.Parse(splitted[0]);
            newRule.SucceeedingNumber = int.Parse(splitted[1]);
            return newRule;
        }

        public bool AppliesToSet(List<int> values)
        {
            return values.Contains(SucceeedingNumber) && values.Contains(PrecedingNumber);
        }

        public List<int> FixSet(List<int> values)
        {
            var newSet = new int[values.Count];
            values.CopyTo(newSet);
            var indexSucceding = values.IndexOf(SucceeedingNumber);
            var indexPreceding = values.IndexOf(PrecedingNumber);
            newSet[indexPreceding] = SucceeedingNumber;
            newSet[indexSucceding] = PrecedingNumber;
            return newSet.ToList(); ;
        }

        public bool IsValidSet(List<int> values)
        {
            return values.IndexOf(SucceeedingNumber) > values.IndexOf(PrecedingNumber);
        }


    }
}