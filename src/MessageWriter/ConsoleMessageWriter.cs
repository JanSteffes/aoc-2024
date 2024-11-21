namespace aoc_2024.MessageWriter
{
    public class ConsoleMessageWriter : IMessageWriter
    {
        public void WriteMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
