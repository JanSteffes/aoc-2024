using System.Drawing;

namespace aoc_2024_unittests
{
    internal class Experiments
    {
        [Test]
        public void CheckPointEquals()
        {
            var q = new Point(1, 1);
            var p = new Point(1, 1);
            Assert.That(q, Is.EqualTo(p));
        }


        [Test]
        public void CheckPointinHashSet()
        {
            var q = new Point(1, 1);
            var dict = new HashSet<Point>
            {
                q
            };
            var p = new Point(1, 1);
            Assert.That(dict.Add(p), Is.False);
        }

        [Test]
        public void CheckPointIsSame()
        {
            var q = new Point(1, 1);
            var p = new Point(1, 1);
            Assert.That(q == p, Is.True);
        }

        [Test]
        public void HashSetAndListIntersect()
        {
            var q = new HashSet<Point> {
            new Point(1,1),
            new Point(1,2)};
            var p = new List<Point>
            {
                new Point(1, 2),
                new Point(1, 3)
            };
            var intersect = q.Intersect(p).First();
            Assert.That(intersect, Is.EqualTo(new Point(1, 2)));
        }

        [Test]
        public void ListContains()
        {
            var q = new List<Point> {
            new(1,1),
            new(1,2)};
            Assert.That(q, Does.Contain(new Point(1, 2)));
        }
    }
}
