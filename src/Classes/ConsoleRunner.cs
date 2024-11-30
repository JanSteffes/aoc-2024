using aoc_2024.Interfaces;
using System.Diagnostics;

namespace aoc_2024.Classes
{
    public class ConsoleRunner : IRunner
    {
        public ExecutionResult Run(ISolution solution, int dayNumber, Part part, string input)
        {
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                string result = part == Part.A ? solution.RunPartA(input) : solution.RunPartB(input);
                sw.Stop();

                return new()
                {
                    Result = result,
                    ResultType = ExecutionResultType.Success,
                    ElapsedTimeInMs = sw.ElapsedMilliseconds,
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Result = ex.Message,
                    ResultType = ExecutionResultType.Failure,
                };
            }
        }
    }
}
