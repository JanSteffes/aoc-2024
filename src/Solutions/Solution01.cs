using aoc_2024.Interfaces;
using System.Text.RegularExpressions;

namespace aoc_2024.Solutions
{
    public class Solution01 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var (firstList, secondList) = GetLists(inputData);

            firstList.Sort();
            secondList.Sort();

            var distance = 0;
            for (var i = 0; i < firstList.Count; i++)
            {
                var firstValue = firstList[i];
                var secondValue = secondList[i];
                distance += Math.Abs(firstValue - secondValue);
            }
            return distance.ToString();
        }

        public string RunPartB(string inputData)
        {
            var (firstList, secondList) = GetLists(inputData);
            var secondListOccurances = secondList.GroupBy(s => s).ToDictionary(s => s.Key, s => s.Key * s.Count());

            var similarityScore = 0;
            foreach (var firstListValue in firstList)
            {
                if (secondListOccurances.ContainsKey(firstListValue))
                {
                    similarityScore += secondListOccurances[firstListValue];
                }
            }
            return similarityScore.ToString();
        }

        private static (List<int> FirstList, List<int> SecondList) GetLists(string inputValues)
        {
            const string numberRegex = "^(\\d+)\\s+(\\d+)$";
            var regexToMatch = new Regex(numberRegex);

            var allValues = inputValues.Split(Environment.NewLine)
                .Select(x => regexToMatch.Matches(x))
                .Select(s => (FirstValue: int.Parse(s[0].Groups[1].Value), SecondValue: int.Parse(s[0].Groups[2].Value))).ToList();
            var firstList = allValues.Select(s => s.FirstValue).ToList();
            var secondList = allValues.Select(s => s.SecondValue).ToList();
            return (firstList, secondList);
        }
    }
}