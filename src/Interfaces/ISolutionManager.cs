namespace aoc_2024.Interfaces
{
    public interface ISolutionManager
    {
        void CreateInitialFiles(int dayToInitialize, string inputContent);
        bool IsDayAlreadyInitialized(int dayToInitialize);
        ISolution? CreateSolutionInstance(int dayToRun);
        string ReadInputFile(int dayNumber);
        int[] GetAvailableSolutions();
    }
}
