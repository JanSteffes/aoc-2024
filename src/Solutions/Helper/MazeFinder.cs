using System.Drawing;

namespace aoc_2024.Solutions.Helper
{
    class MazeFinder<T>
    {
        public MazeMap Maze;

        public MazeFinder(MazeMap map)
        {
            Maze = map;
        }

        public List<ValuePoint<char>> GetPathAStar(bool printMapToConsole)
        {
            var categories = Maze.GetValuePointCategories();
            var start = categories.First(c => c.Name.Contains("start", StringComparison.OrdinalIgnoreCase)).ValuePoints.First();
            var end = categories.First(c => c.Name.Contains("end", StringComparison.OrdinalIgnoreCase)).ValuePoints.First();
            var freeFieldsCategory = categories.First(c => c.Name.Contains("free", StringComparison.OrdinalIgnoreCase));
            CalculateHValue(freeFieldsCategory, end);
            freeFieldsCategory.Add(end);
            freeFieldsCategory.Add(start);

            var openFields = new PriorityQueue<ValuePoint<char>, int>();
            openFields.Enqueue(start, 0);

            do
            {
                var currentNode = openFields.Dequeue();

                if (printMapToConsole)
                {
                    var dict = GetColorDict(currentNode, freeFieldsCategory);
                    Maze.PrintColoredMap(dict);
                    Thread.Sleep(500);
                }

                if (currentNode == end)
                {
                    return currentNode.GetPath();
                }

                currentNode.State = NodeState.Closed;
                ExpandNode(currentNode, freeFieldsCategory, openFields, end);

            } while (openFields.Count > 0);
            return [];
        }

        private IDictionary<Point, ConsoleColor>? GetColorDict(ValuePoint<char> currentNode, ValuePointCategory<char> freeFieldsCategory)
        {
            var dict = new Dictionary<Point, ConsoleColor>
                {
                    {currentNode.Coordinate, ConsoleColor.White },
                };
            foreach (var value in freeFieldsCategory.ValuePoints)
            {
                if (value.Coordinate == currentNode.Coordinate)
                {
                    continue;
                }
                var color = value.State switch
                {
                    NodeState.Closed => ConsoleColor.DarkGray,
                    NodeState.Open => ConsoleColor.DarkGreen,
                    NodeState.Untested => ConsoleColor.DarkYellow,
                    _ => throw new NotImplementedException(),
                };
                dict.Add(value.Coordinate, color);
            }
            return dict;
        }

        private void ExpandNode(ValuePoint<char> currentNode, ValuePointCategory<char> freeFieldsCategory, PriorityQueue<ValuePoint<char>, int> openFields, ValuePoint<char> end)
        {
            foreach (var nextNodePoint in currentNode.Coordinate.GetDirectPointsAround())
            {
                if (!Maze.IsInMap(nextNodePoint) || !freeFieldsCategory.ContainsPoint(nextNodePoint))
                {
                    continue;
                }

                var nextNode = freeFieldsCategory.GetValuePointByPoint(nextNodePoint);
                if (nextNode.State == NodeState.Closed)
                {
                    continue;
                }

                var tempG = currentNode.G + nextNode.GetTraversalCost();
                var openListContainsNextNode = openFields.UnorderedItems.ToHashSet().Any(a => a.Element == nextNode);
                if (openListContainsNextNode && tempG > nextNode.G)
                {
                    continue;
                }

                nextNode.ParentNode = currentNode;
                nextNode.G = tempG;

                if (!openListContainsNextNode)
                {
                    openFields.Enqueue(nextNode, nextNode.F);
                }
                else
                {
                    openFields.Remove(nextNode, out _, out _);
                    openFields.Enqueue(nextNode, nextNode.F);
                }
            }
        }

        private void CalculateHValue(ValuePointCategory<char> freeFieldsCategory, ValuePoint<char> end)
        {
            foreach (var freeField in freeFieldsCategory.ValuePoints)
            {
                freeField.H = Math.Abs(freeField.Coordinate.X - end.Coordinate.X) + Math.Abs(freeField.Coordinate.Y - end.Coordinate.Y);
            }
        }
    }
}