namespace aoc_2024.Interfaces
{
    public interface IRunner
    {
        string RunDay(ISolution solution, int dayNumber, Part part, string input);
        void TestDay(int dayNumber, Part part, int testNumber);
        void InitializeDay(int dayNumber);
    }
}
