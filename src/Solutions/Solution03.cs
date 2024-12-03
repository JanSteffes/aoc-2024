using aoc_2024.Interfaces;
using System.Text.RegularExpressions;

namespace aoc_2024.Solutions
{
    public class Solution03 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var regexToMatch = new Regex("(?:mul\\((\\d+),(\\d+)\\))");
            var regexMatches = regexToMatch.Matches(inputData);
            var sum = regexMatches.Sum(i => int.Parse(i.Groups[1].Value) * int.Parse(i.Groups[2].Value));
            return sum.ToString();
        }

        public string RunPartB(string inputData)
        {
            var regexString = "(?:mul\\((\\d+),(\\d+)\\)|(don't\\(\\))|(do\\(\\)))";
            var keyWordRegex = new Regex(regexString);
            var regexMatches = keyWordRegex.Matches(inputData);
            var steps = regexMatches.Select(match => new Step(match)).ToList();
            var stopped = false;
            var sum = 0;
            for (var index = 0; index < steps.Count(); index++)
            {
                var currentStep = steps[index];
                switch (currentStep.StepType)
                {
                    case StepType.Operation:
                        if (stopped)
                        {
                            continue;
                        }
                        sum += currentStep.Value;
                        break;
                    case StepType.Stop:
                        if (!stopped)
                        {
                            stopped = true;
                        }
                        break;
                    case StepType.Start:
                        if (stopped)
                        {
                            stopped = false;
                        }
                        break;
                }
            }
            return sum.ToString();
        }

        internal class Step
        {
            private Match match;

            internal StepType StepType { get; private set; }

            public Step(Match match)
            {
                this.match = match;
                StepType = !string.IsNullOrEmpty(match.Groups[3].Value) ? StepType.Stop :
                    !string.IsNullOrEmpty(match.Groups[4].Value) ? StepType.Start : StepType.Operation;
            }

            public int Value => StepType == StepType.Operation ? int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value) : throw new InvalidOperationException($"Can't get value on type {StepType}!");
        }

        internal enum StepType
        {
            Operation,
            Stop,
            Start
        }


    }
}