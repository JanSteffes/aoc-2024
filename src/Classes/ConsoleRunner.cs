using aoc_2024.Interfaces;
using System.Diagnostics;

namespace aoc_2024.Classes
{
    public class ConsoleRunner : IRunner
    {
        private readonly ILogger logger;

        public ConsoleRunner(ILogger logger)
        {
            this.logger = logger;
        }

        public void InitializeDay(int dayNumber)
        {
            throw new NotImplementedException();
        }

        public string RunDay(ISolution solution, int dayNumber, Part part, string input)
        {
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                string result = part == Part.A ? solution.RunPartA(input) : solution.RunPartB(input);
                Thread.Sleep(1000);
                sw.Stop();
                logger.Log($"Day #{dayNumber} - Part {part} successfully run!", LogSeverity.Runner);
                logger.Log($"Result: {result}", LogSeverity.Runner);
                logger.Log($"Time taken: {sw.Elapsed.TotalMilliseconds} ms", LogSeverity.Runner);
                return result;
            }
            catch (Exception ex)
            {
                logger.Log($"Error runnning Day #{dayNumber} - Part {part}:", LogSeverity.Error);
                logger.Log(ex.Message, LogSeverity.Other);
            }

            return string.Empty;
        }

        public void TestDay(int dayNumber, Part part, int testNumber)
        {
            //WriteLastChoice(dayNumber, Mode.Test, part, testNumber);
        }
    }
}
