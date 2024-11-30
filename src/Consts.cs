namespace aoc_2024
{
    public static class Consts
    {
        public const int year = 2023;
        public const string baseUri = "https://adventofcode.com";
    }

    public enum Mode
    {
        Repeat,
        Run,
        Test,
        Init,
        Exit
    }

    public enum Part
    {
        A,
        B
    }

    public enum ClientResponseType
    {
        Success,
        Failure,
    }

    public enum LogSeverity
    {
        Log,
        Error,
        Runner,
        Other
    }
}
