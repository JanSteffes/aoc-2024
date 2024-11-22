namespace aoc_2024.AocClient
{
    public interface IAocClient
    {
        Task<ClientResponse> GetPuzzleInput(int dayNumber);
    }
}
