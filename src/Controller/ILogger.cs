namespace aoc_2024.Controller
{
    public interface ILogger
    {
        void Log(string message, LogSeverity logSeverity);
    }
}
