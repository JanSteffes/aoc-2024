namespace aoc_2024.Classes
{
    public enum ExecutionResultType
    {
        Success,
        Failure,
    }

    public class ExecutionResult
    {
        public string Result { get; set; } = string.Empty;
        public long ElapsedTimeInMs { get; set; }
        public ExecutionResultType ResultType { get; set; }
    }
}
