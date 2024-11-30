using aoc_2024.AocClient;

namespace aoc_2024.Interfaces
{
    public interface IAocClient
    {
        Task<ClientResponse> GetPuzzleInput(int dayNumber);
    }
}
