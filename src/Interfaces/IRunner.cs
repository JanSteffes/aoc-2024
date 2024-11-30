using aoc_2024.Classes;

namespace aoc_2024.Interfaces
{
    public interface IRunner
    {
        ExecutionResult Run(ISolution solution, int dayNumber, Part part, string input);
    }
}
