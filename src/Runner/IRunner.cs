namespace aoc_2024.Runner
{
    public interface IRunner
    {
        void RunDay(int dayNumber, Part part);
        void TestDay(int dayNumber, Part part, int testNumber);
        void InitializeDay(int dayNumber);
    }
}
