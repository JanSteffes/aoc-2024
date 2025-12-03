using aoc_2024.Interfaces;
using aoc_2024.Solutions.Helper;
using System.Drawing;

namespace aoc_2024.Solutions
{
    public class Solution20 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var raceTrackMap = new MazeMap(inputData, true);
            //raceTrackMap.PrintColoredMap();
            var finder = new MazeFinder<char>(raceTrackMap);
            var path = finder.GetPathAStar(false);
            var categories = raceTrackMap.GetValuePointCategories();
            var obstalces = categories.First(f => f.Name.Contains("obs", StringComparison.OrdinalIgnoreCase));
            var freeFields = categories.First(f => f.Name.Contains("free", StringComparison.OrdinalIgnoreCase));
            var startPoint = categories.First(f => f.Name.Contains("start", StringComparison.OrdinalIgnoreCase)).ValuePoints.First();
            var endPoint = categories.First(f => f.Name.Contains("end", StringComparison.OrdinalIgnoreCase)).ValuePoints.First();
            //freeFields.Add(startPoint);
            //freeFields.Add(endPoint);
            // foreach startfield and freefield, find walls along path 
            var cheatableFieldsX = freeFields.ValuePoints.Where(ff => obstalces.ContainsPoint(ff.Coordinate.NewPointFromVector(1, 0)) && freeFields.ContainsPoint(ff.Coordinate.NewPointFromVector(2, 0)))
                .Select(cff => cff.Coordinate.NewPointFromVector(1, 0))
                .ToList();
            var cheatableFieldsY = freeFields.ValuePoints.Where(ff => obstalces.ContainsPoint(ff.Coordinate.NewPointFromVector(0, 1)) && freeFields.ContainsPoint(ff.Coordinate.NewPointFromVector(0, 2)))
                .Select(cff => cff.Coordinate.NewPointFromVector(0, 1))
                .ToList();

            //PrintCheatableFields(raceTrackMap, cheatableFreeFieldsX, cheatableFreeFieldY);

            // group by x? group by y?
            var purePointsPath = path.Select(p => p.Coordinate).ToList();
            var savedSecondsForX = GetSavedSecondsForCheatFields(purePointsPath, cheatableFieldsX, new Point(-1, 0), raceTrackMap);
            var savedSecondsForY = GetSavedSecondsForCheatFields(purePointsPath, cheatableFieldsY, new Point(0, -1), raceTrackMap);
            var savedSecondGroups = savedSecondsForX.Concat(savedSecondsForY).GroupBy(q => q.Value).OrderBy(q => q.Key).ToList();
            //);
            //var savedSecondGroups = savedSeconds.GroupBy(g => g.Value).OrderBy(q => q.Key).ToList();
            foreach (var group in savedSecondGroups)
            {
                Console.WriteLine($"There are {group.Count()} cheats to save {group.Key} picoseconds");
            }
            return savedSecondGroups.Where(s => s.Key >= 100).Sum(q => q.Count()).ToString();
        }

        private Dictionary<Point, int> GetSavedSecondsForCheatFields(List<Point> path, List<Point> cheatableFields, Point vectorToStartField, MazeMap raceTrackMap)
        {
            var resultDict = new Dictionary<Point, int>();
            foreach (var cheatableField in cheatableFields)
            {
                var startFieldCoordinates = cheatableField.NewPointFromVector(vectorToStartField);
                var startFieldIndex = path.IndexOf(cheatableField.NewPointFromVector(vectorToStartField));
                var endFieldCoordinates = cheatableField.NewPointFromVector(-vectorToStartField.X, -vectorToStartField.Y);
                var endFieldIndex = path.IndexOf(endFieldCoordinates);
                var coloredFields = new List<Point> { startFieldCoordinates, endFieldCoordinates }.ToDictionary(q => q, q => ConsoleColor.DarkBlue);
                //raceTrackMap.PrintColoredMap(coloredFields);
                resultDict.Add(cheatableField, Math.Abs(endFieldIndex - startFieldIndex) - 2);
            }
            return resultDict;
        }

        private static void PrintCheatableFields(MazeMap raceTrackMap, List<Point> cheatableFreeFieldsX, List<Point> cheatableFreeFieldY)
        {
            var colorFieldsX = cheatableFreeFieldsX.ToDictionary(d => d, d => ConsoleColor.Yellow);
            raceTrackMap.PrintColoredMap(colorFieldsX);
            var colorFieldsY = cheatableFreeFieldY.ToDictionary(d => d, d => ConsoleColor.DarkBlue);
            raceTrackMap.PrintColoredMap(colorFieldsY);
        }

        private static List<ValuePoint<char>> ProcessMapWithCheatableField(string inputData, List<ValuePointCategory<char>> categories, ValuePointCategory<char> obstalces, Point cheatableField)
        {
            var newRaceTrackMap = new MazeMap(inputData, true);
            newRaceTrackMap.ConvertField(cheatableField, "obs", "free");
            var newFinder = new MazeFinder<char>(newRaceTrackMap);
            var newPath = newFinder.GetPathAStar(false);
            //newRaceTrackMap.PrintColoredMap(newPath.ToDictionary(p => p.Coordinate, p => ConsoleColor.Red));
            return newPath;
        }

        public string RunPartB(string inputData)
        {
            throw new NotImplementedException();
        }
    }
}