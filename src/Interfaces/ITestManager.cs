using aoc_2024.Classes;

namespace aoc_2024.Interfaces
{
    public interface ITestManager
    {
        List<TestCase> Parse(int dayNumber);
        int[] GetAvailableTests();
    }
}
