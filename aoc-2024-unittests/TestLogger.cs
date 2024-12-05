using aoc_2024;
using aoc_2024.Interfaces;
using System.Diagnostics;

namespace aoc_2024_unittests
{
    internal class TestLogger : ILogger
    {
        public void Log(string message, LogSeverity logSeverity)
        {
            Debug.WriteLine(message);
        }
    }
}
