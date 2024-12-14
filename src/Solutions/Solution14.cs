using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;

namespace aoc_2024.Solutions
{
    public class Solution14 : ISolution
    {
        private const int numberOfItemsForValidLine = 10;

        public string RunPartA(string inputData)
        {
            var isTestCase = inputData.Length == 170;
            var boundX = isTestCase ? 11 : 101;
            var boundY = isTestCase ? 7 : 103;
            var middleX = ((boundX + 1) / 2) - 1;
            var middleY = ((boundY + 1) / 2) - 1;
            var robots = ParseUtils.ParseIntoLines(inputData).Select(Robot.FromLine).ToList();
            var quadrants = robots.GroupBy(r => GetQuadrantForPosition(r.GetPosition(), middleX, middleY)).ToDictionary(d => d.Key, d => d.ToList());
            //PrintQuadrants(quadrants, boundX, boundY, middleX, middleY);
            for (var i = 0; i < 100; i++)
            {
                robots.ForEach(r => r.MoveOnMap(boundX, boundY));
            }
            var quadrantsAfterMoves = robots.GroupBy(r => GetQuadrantForPosition(r.GetPosition(), middleX, middleY)).ToDictionary(d => d.Key, d => d.ToList());
            //Console.WriteLine(Environment.NewLine);
            //PrintQuadrants(quadrantsAfterMoves, boundX, boundY, middleX, middleY);
            var result = 1;
            foreach (var quadrant in quadrantsAfterMoves.Where(q => q.Key != -1))

            {
                result *= quadrant.Value.Count;
            }
            return result.ToString();


            static int GetQuadrantForPosition(Point position, int middleX, int middleY)
            {
                var x = position.X;
                var y = position.Y;
                if (x < middleX && y < middleY)
                {
                    return 1;
                }
                if (x > middleX && y < middleY)
                {
                    return 2;
                }
                if (x < middleX && y > middleY)
                {
                    return 3;
                }
                if (x > middleX && y > middleY)
                {
                    return 4;
                }
                return -1;
            }

            static void PrintQuadrants(Dictionary<int, List<Robot>> quadrants, int boundX, int boundY, int middleX, int middleY)
            {
                ConsoleColor[] quadrantColors = [ConsoleColor.Green, ConsoleColor.Red, ConsoleColor.Blue, ConsoleColor.Magenta];
                for (int y = 0; y < boundY; y++)
                {
                    Console.WriteLine();
                    for (int x = 0; x < boundX; x++)
                    {
                        try
                        {
                            var currentPosition = new Point(x, y);
                            var quadrant = GetQuadrantForPosition(currentPosition, middleX, middleY);
                            var middle = quadrant == -1;
                            var color = middle ? ConsoleColor.White : quadrantColors[quadrant - 1];
                            var prevColor = Console.ForegroundColor;
                            Console.ForegroundColor = color;
                            var robotsInPosition = quadrants.TryGetValue(quadrant, out var robotsInQuadrant) ? robotsInQuadrant.Where(p => p.GetPosition() == currentPosition).Count() : 0;
                            var charToWrite = robotsInPosition > 0 ? robotsInPosition.ToString() : ".";
                            Console.Write(charToWrite);
                            Console.ForegroundColor = prevColor;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                        }

                    }
                }
            }


        }

        public string RunPartB(string inputData)
        {
            const string imageSavePath = "C:\\Users\\JanSt\\OneDrive\\Dokumente\\AoC2024\\14";
            var allImagesPath = Path.Combine(imageSavePath, "all");
            Directory.Delete(imageSavePath, true);
            Directory.CreateDirectory(imageSavePath);
            Directory.CreateDirectory(allImagesPath);
            var isTestCase = inputData.Length == 170;
            var boundX = isTestCase ? 11 : 101;
            var boundY = isTestCase ? 7 : 103;
            var middleX = ((boundX + 1) / 2) - 1;
            var middleY = ((boundY + 1) / 2) - 1;
            var robots = ParseUtils.ParseIntoLines(inputData).Select(Robot.FromLine).ToList();
            for (var i = 0; i < 1000000; i++)
            {
                Debug.WriteLine($"Run {i + 1}");
                if (TreeFound(robots))
                {
                    PrintToImage(robots, boundX, boundY, i, false);
                }
                robots.ForEach(r => r.MoveOnMap(boundX, boundY));
            }
            return "NotFound!";

            bool TreeFound(List<Robot> robots)
            {
                // get groups of ys
                var groupedByY = robots.GroupBy(g => g.GetPosition().Y).ToDictionary(g => g.Key, g => g.ToList());
                // find lines in ys
                var xLines = DetectLines(groupedByY, true);
                // more than one valid line? could be a tree!
                return xLines.Count > 1;
            }

            List<List<Point>> DetectLines(Dictionary<int, List<Robot>> groupedByY, bool xLine)
            {
                var lines = new List<List<Point>>();
                var relevantGroups = groupedByY.Where(g => g.Value.Count > numberOfItemsForValidLine).ToList();
                foreach (var group in relevantGroups)
                {
                    var orderd = group.Value.OrderBy(p => p.GetPosition().X).ToList();
                    var current = new List<Point>();
                    foreach (var robot in orderd)
                    {
                        if (current.Count == 0)
                        {
                            current.Add(robot.GetPosition());
                        }
                        else
                        {
                            if (current.Last().X + 1 == robot.GetPosition().X)
                            {
                                current.Add(robot.GetPosition());
                            }
                            else
                            {
                                if (current.Count > numberOfItemsForValidLine)
                                {
                                    lines.Add(current);
                                }
                                current = [];
                            }
                        }
                    }
                }
                return lines;
            }

            void PrintToImage(List<Robot> robots, int boundX, int boundY, int step, bool saveToAll)
            {
                var myList = robots.ToList();
                var bitmap = new Bitmap(boundX, boundY);
                for (int y = 0; y < boundY; y++)
                {
                    for (int x = 0; x < boundX; x++)
                    {
                        var matchingRobots = myList.Where(p => p.GetPosition() == new Point(x, y)).ToList();
                        var color = matchingRobots.Any() ? Color.White : Color.Black;
                        bitmap.SetPixel(x, y, color);
                        myList = myList.Except(matchingRobots).ToList();

                    }
                }
                var fileName = $"{step}.png";
                var fullFilePath = saveToAll ? Path.Combine(allImagesPath, fileName)
                    : Path.Combine(imageSavePath, fileName);
                var newBitmap = new Bitmap(bitmap, bitmap.Size.Width * 1, bitmap.Size.Height * 1);
                //stuff.Post((newBitmap, fullFilePath));
                Task.Factory.StartNew(() =>
                {
                    newBitmap.Save(fullFilePath, System.Drawing.Imaging.ImageFormat.Png);
                    newBitmap.Dispose();
                    bitmap.Dispose();
                });
            }



            static async Task WriteBitmaptask(ISourceBlock<(Bitmap bitmapToWrite, string filePath)> bitmapsToWrite)
            {
                while (await bitmapsToWrite.OutputAvailableAsync())
                {
                    var bitMapToWrite = await bitmapsToWrite.ReceiveAsync();
                    bitMapToWrite.bitmapToWrite.Save(bitMapToWrite.filePath);
                }
            }
        }

        internal class Robot
        {
            private const string ParseRegex = "(?:p=(?<positionX>\\d+),(?<positionY>\\d+)) (?:v=(?<vectorX>-?\\d+),(?<vectorY>-?\\d+))";

            private Point Position { get; set; }

            private int VectorX { get; set; }

            private int VectorY { get; set; }

            public Robot(int x, int y)
            {
                Position = new Point(x, y);
            }

            private Robot(Point position, int vectorX, int vectorY)
            {
                Position = position;
                VectorX = vectorX;
                VectorY = vectorY;
            }

            public Point GetPosition()
            {
                return Position;
            }

            internal static Robot FromLine(string arg1)
            {
                var match = new Regex(ParseRegex).Match(arg1);
                var positionX = int.Parse(match.Groups["positionX"].Value);
                var positionY = int.Parse(match.Groups["positionY"].Value);
                var position = new Point(positionX, positionY);
                var vectorX = int.Parse(match.Groups["vectorX"].Value);
                var vectorY = int.Parse(match.Groups["vectorY"].Value);
                return new Robot(position, vectorX, vectorY);
            }

            public void MoveOnMap(int boundX, int boundY)
            {
                var newX = MoveDirection(boundX, Position.X, VectorX);
                var newY = MoveDirection(boundY, Position.Y, VectorY);
                Position = new Point(newX, newY);
            }

            private static int MoveDirection(int bound, int current, int vector)
            {
                var newPos = current + vector;
                if (newPos < 0)
                {
                    newPos = bound - Math.Abs(newPos);
                }
                if (newPos >= bound)
                {
                    newPos -= bound;
                }
                return newPos;
            }

            public override string ToString()
            {
                return $"{Position.X}|{Position.Y} with {VectorX}/{VectorY}";
            }
        }
    }
}