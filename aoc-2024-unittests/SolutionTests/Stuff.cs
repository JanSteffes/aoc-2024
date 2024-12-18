

using aoc_2024.Solutions;
using System.Diagnostics;

namespace aoc_2024_unittests.SolutionTests
{
    internal class Stuff
    {
        [TestCase(6561)]
        public void TestProgram(int value)
        {
            var input = @"Register A: 2024
Register B: 0
Register C: 0

Program: 0,3,5,4,3,0";
            var programm = new Program(input);

            var instructions = programm.Instructions;
            instructions.Reverse();

            programm.A = value;
            programm.Run();
            var result = programm.Output;
            Trace.WriteLine(result);
        }
    }
}
