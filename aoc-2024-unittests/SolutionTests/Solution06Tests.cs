using aoc_2024.Solutions;

namespace aoc_2024_unittests.SolutionTests
{
    [TestFixture]
    internal class Solution06Tests
    {
        [Test]
        public void Part1A()
        {
            // arrange
            var testManager = new UnitTestManager(new TestLogger());
            var testCasesForDay = testManager.Parse(6);
            var testCase = testCasesForDay.First(test => test.TestNumber == 1);
            var expected = testCase.AnswerA;
            var input = testCase.Input;
            var solution6 = new Solution06();

            // act
            var result = solution6.RunPartA(input);

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
