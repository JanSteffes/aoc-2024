using aoc_2024.Interfaces;
using System.Diagnostics;

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
        [TestCase("9", 1, "A")]
        [TestCase("9", 1, "B")]
        [TestCase("10", 1, "A")]
        [TestCase("10", 1, "B")]
        [TestCase("11", 1, "A")]
        //[TestCase("11", 1, "B")]  // skip for now, takes too long
        [TestCase("12", 1, "A")]
        [TestCase("12", 1, "B")]
        [TestCase("13", 1, "A")]
        [TestCase("13", 1, "B")]
        public void TestCaseTests(int day, int testNumber, string part)
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

        [TestCase("1", 1, "A", "1590491", 1)]
        [TestCase("1", 1, "B", "22588371", 1)]
        [TestCase("2", 1, "A", "502", 1)]
        [TestCase("2", 1, "B", "544", 1)]
        [TestCase("3", 1, "A", "175700056", 1)]
        [TestCase("3", 2, "B", "71668682", 1)]
        [TestCase("4", 1, "A", "2504", 1)]
        [TestCase("4", 1, "B", "1923", 1)]
        [TestCase("5", 1, "A", "5275", 1)]
        [TestCase("5", 1, "B", "6191", 1)]
        [TestCase("6", 1, "A", "4559", 1)]
        [TestCase("6", 1, "B", "1604", 20)]
        [TestCase("7", 1, "A", "1298103531759", 3)]
        //[TestCase("7", 1, "B", "140575048428831", 90)] // skip for now, takes too long
        [TestCase("8", 1, "A", "426", 1)]
        [TestCase("8", 1, "B", "1359", 1)]
        [TestCase("9", 1, "A", "6211348208140", 1)]
        [TestCase("9", 1, "B", "6239783302560", 2)]
        [TestCase("10", 1, "A", "760", 1)]
        [TestCase("10", 1, "B", "1764", 1)]
        [TestCase("11", 1, "A", "183435", 2)]
        //[TestCase("11", 1, "B", "218279375708592", 1)] // skip for now, takes too long
        [TestCase("12", 1, "A", "1344578", 1)]
        [TestCase("12", 1, "B", "814302", 1)]
        [TestCase("13", 1, "A", "", 1)]
        [TestCase("13", 1, "B", "", 1)]
        public void InputCaseTests(int day, int testNumber, string part, string expectedResult, int maxSecondsToRun)
        {
            // arrange
            var solutionClass = GetSolutionClass(day);

            var testManager = new InputTestManager(new TestLogger());
            var testCase = testManager.Parse(day);
            var input = testCase.First().Input;

            // act
            var sw = Stopwatch.StartNew();
            var result = part == "A" ? solutionClass.RunPartA(input) : solutionClass.RunPartB(input);
            sw.Stop();

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(expectedResult));
                Assert.That(sw.Elapsed.TotalSeconds, Is.LessThanOrEqualTo(maxSecondsToRun));
            });
        }

        private static ISolution GetSolutionClass(int day)
        {
            var aoc2024Assembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "aoc-2024");

            var typeToSearchFor = "Solution" + (day > 9 ? day : "0" + day);
            var solutionType = aoc2024Assembly.GetTypes().First(t => t.Name.Contains(typeToSearchFor));
            var instanceOftype = (ISolution)(Activator.CreateInstance(solutionType) ?? throw new Exception($"Failed to create instancer of type {solutionType.Name}!"));
            return instanceOftype;
        }
    }
}
