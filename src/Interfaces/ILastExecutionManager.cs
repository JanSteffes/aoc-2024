namespace aoc_2024.Interfaces
{
    public struct ExecutionSettings
    {
        public Mode mode;
        public int day;
        public Part part;
        public int testNumber;
    }

    public interface ILastExecutionManager
    {
        void LoadLastExecution();
        ExecutionSettings GetLastExecution();
        public void WriteLastChoice(int dayNumber, Mode mode, Part part, int? testNumber = null);
        bool HasLastExecution();
    }
}
