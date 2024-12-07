using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace aoc_2024.Solutions
{
    public class Solution07 : ISolution
    {
        private readonly string[] partASymbols = ["+", "*"];

        private readonly string[] partBSymbols = ["+", "*", "|"];

        public string RunPartA(string inputData)
        {
            // 1298103531759
            return RunWithSymbols(inputData, partASymbols);

        }


        public string RunPartB(string inputData)
        {
            // 140575048428831
            return RunWithSymbols(inputData, partBSymbols);
        }

        public string RunWithSymbols(string inputData, string[] symbolsToUse)
        {
            var equations = ParseUtils.ParseIntoLines(inputData).Select(s => new Equation(s)).ToList();
            var validResult = new ConcurrentBag<(long CalculationResult, int PermutationsProcessed)>();
            using var _ = StartLogTimer(validResult, equations.Count);
            Parallel.ForEach(equations, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, equation =>
            {
                var result = ProcessEquation(equation, symbolsToUse);
                validResult.Add(result);
            });
            var sum = validResult.Sum(v => v.CalculationResult);
            return sum.ToString();
        }

        private System.Timers.Timer StartLogTimer(ConcurrentBag<(long CalculationResult, int PermutationsProcessed)> itemsToLog, int maxItems)
        {
            var timer = new System.Timers.Timer();
            var startTime = DateTime.Now;
            timer.Interval = 2000;
            timer.Elapsed += delegate { LogStuff(startTime, itemsToLog, maxItems); };
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Start();
            return timer;
        }

        private static void LogStuff(DateTime startTime, ConcurrentBag<(long CalculationResult, int PermutationsProcessed)> itemsToLog, int maxItems)
        {
            Debug.WriteLine($"Processed {itemsToLog.Count}/{maxItems} points and detected {itemsToLog.Count(i => i.CalculationResult != 0)} valid results. Used ~{(int)itemsToLog.Average(i => i.PermutationsProcessed / maxItems)} permuations per Item  [Running for {((int)(DateTime.Now - startTime).TotalSeconds)} seconds]");
        }
        private (long CalculationResult, int PermutationsProcessed) ProcessEquation(Equation equation, string[] symbols)
        {
            var dataTable = new DataTable();
            var permutations = new HashSet<string>();
            var possiblePermutations = Math.Pow(symbols.Length, equation.ValuesToCombine.Length - 1);
            var random = new Random();
            do
            {
                var randomSymbolList = string.Join("", random.GetItems(symbols, equation.ValuesToCombine.Length - 1));
                if (!permutations.Add(randomSymbolList))
                {
                    continue;
                }
                var lastResult = equation.ValuesToCombine.First();
                var fullCalculationString = lastResult.ToString();
                for (int index = 0; index < equation.ValuesToCombine.Length - 1; index++)
                {
                    var symbolToUse = randomSymbolList[index];
                    var firstValue = lastResult;
                    var secondValue = equation.ValuesToCombine[index + 1];
                    fullCalculationString += " " + symbolToUse + " " + secondValue.ToString();
                    string currentResultString;
                    if (symbolToUse == '|')
                    {
                        currentResultString = long.Parse(firstValue.ToString() + secondValue.ToString()).ToString(); ;
                    }
                    else
                    {
                        var equationToCompute = firstValue.ToString() + ".0" + symbolToUse + secondValue.ToString() + ".0";
                        var equationResultString = dataTable.Compute(equationToCompute, null).ToString();
                        if (string.IsNullOrEmpty(equationToCompute))
                        {
                            throw new ArithmeticException($"Could not calculate '{equationToCompute}'!");
                        }
                        currentResultString = equationResultString!;
                    }
                    lastResult = long.Parse(currentResultString.Split(",").First());
                    if (lastResult > equation.ExpectedResult)
                    {
                        break;
                    }

                }
                if (lastResult == equation.ExpectedResult)
                {
                    return (equation.ExpectedResult, permutations.Count);
                }
            }
            while (permutations.Count < possiblePermutations);
            return (0, permutations.Count);
        }

    }

    internal class Equation
    {
        private const string ParseRegex = "^(?<expectedResult>\\d+):(?<values> \\d+)+$";

        public long ExpectedResult { get; set; }

        public long[] ValuesToCombine { get; set; }

        public Equation(string line)
        {
            var regexToParse = new Regex(ParseRegex);
            var regexResult = regexToParse.Match(line);
            ExpectedResult = long.Parse(regexResult.Groups["expectedResult"].Value);
            ValuesToCombine = regexResult.Groups["values"].Captures.Select(c => long.Parse(c.Value)).ToArray();
        }
    }
}