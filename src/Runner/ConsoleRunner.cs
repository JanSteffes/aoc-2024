using aoc_2024.MessageWriter;

namespace aoc_2024.Runner
{
    public class ConsoleRunner : IRunner
    {
        private readonly IMessageWriter messageWriter;

        public ConsoleRunner(IMessageWriter messageWriter)
        {
            this.messageWriter = messageWriter;
        }

        public void InitializeDay(int dayNumber)
        {
            throw new NotImplementedException();
        }

        public void RunDay(int dayNumber, Part part)
        {
            //WriteLastChoice(dayNumber, Mode.Run, part);
        }

        public void TestDay(int dayNumber, Part part, int testNumber)
        {
            //WriteLastChoice(dayNumber, Mode.Test, part, testNumber);
        }
    }
}
