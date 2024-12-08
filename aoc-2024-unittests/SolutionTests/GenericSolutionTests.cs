using aoc_2024.Interfaces;

namespace aoc_2024_unittests.SolutionTests
{
    [TestFixture]
    internal class GenericSolutionTests
    {
        [TestCase("1", 1, "A")]
        [TestCase("1", 1, "B")]
        [TestCase("2", 1, "A")]
        [TestCase("2", 1, "B")]
        [TestCase("3", 1, "A")]
        [TestCase("3", 2, "B")]
        [TestCase("4", 1, "A")]
        [TestCase("4", 1, "B")]
        [TestCase("5", 1, "A")]
        [TestCase("5", 1, "B")]
        [TestCase("6", 1, "A")]
        [TestCase("6", 1, "B")]
        [TestCase("7", 1, "A")]
        [TestCase("7", 1, "B")]
        [TestCase("8", 1, "A")]
        [TestCase("8", 1, "B")]
        public void TestSolution(int day, int testNumber, string part)
        {
            // arrange
            var solutionClass = GetSolutionClass(day);

            var testManager = new UnitTestManager(new TestLogger());
            var testCasesForDay = testManager.Parse(day);
            var testCase = testCasesForDay.First(test => test.TestNumber == testNumber);
            var expected = part == "A" ? testCase.AnswerA : testCase.AnswerB;
            var input = testCase.Input;

            // act
            var result = part == "A" ? solutionClass.RunPartA(input) : solutionClass.RunPartB(input);

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }

        private ISolution GetSolutionClass(int day)
        {
            var aoc2024Assembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "aoc-2024");

            var typeToSearchFor = "Solution" + (day > 9 ? day : "0" + day);
            var solutionType = aoc2024Assembly.GetTypes().First(t => t.Name.Contains(typeToSearchFor));
            //var solutionType = aoc2024Assembly.GetType(typeToSearchFor) ?? throw new Exception($"Could not find type {typeToSearchFor} in assembly {aoc2024Assembly.GetName().Name}");
            var instanceOftype = (ISolution)(Activator.CreateInstance(solutionType) ?? throw new Exception($"Failed to create instancer of type {solutionType.Name}!"));
            return instanceOftype;
        }
    }
}
