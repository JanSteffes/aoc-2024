using aoc_2024.Solutions;

namespace aoc_2024_unittests
{
    internal class MapBlockTests
    {

        [TestCase(1, 5, 1, 5)]
        [TestCase(1, 5, 2, 15)]
        [TestCase(2, 5, 2, 25)]
        [TestCase(10, 5, 5, 300)]
        public void TestSumCalc(int index, int value, int range, int expectedResult)
        {
            // arrange
            var mapBlock = new MapBlock(value, range);

            // act
            var result = mapBlock.CalculateValue(index);

            // assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

    }
}
