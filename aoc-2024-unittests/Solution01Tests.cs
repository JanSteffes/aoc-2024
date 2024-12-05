using aoc_2024.Classes;
using aoc_2024.Solutions;

namespace aoc_2024_unittests
{
    [TestFixture]
    public class Solution01Tests
    {
        [Test]
        public void TestA()
        {
            //arrange
            var testCase = new TestManager(new TestLogger()).Parse(1).First(test => test.TestNumber == 1);
            var expected = testCase.AnswerA;
            var input = testCase.Input;
            var solution1 = new Solution01();

            // act
            var result = solution1.RunPartA(input);

            // assert
            Assert.That(result, Is.EqualTo(expected));

        }
    }
}
