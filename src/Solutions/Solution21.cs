using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;
using System.Drawing;

namespace aoc_2024.Solutions
{
    public class Solution21 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var numericKeyPadInDepressurized = new NumericKeyPad();
            var directionalKeyPadInRadiaton = new DirectionalKeyPad(numericKeyPadInDepressurized);
            var directionKeyPadInFreezing = new DirectionalKeyPad(directionalKeyPadInRadiaton);
            //var valuesToPress = "029A".Select(directionKeyPadInFreezing.GetMovesNeededToMoveAndPressValue).SelectMany(q => q).ToArray();
            var stringsToEnter = ParseUtils.ParseIntoLines(inputData);
            var sum = 0;
            foreach (var stringToEnter in stringsToEnter)
            {
                var valuesToPressOnNumericPad = GetValuesToPressForString(numericKeyPadInDepressurized, stringToEnter);
                var valuesToPressOnFirstDirectionalKeyPad = GetValuesToPressForString(directionalKeyPadInRadiaton, stringToEnter);
                var valuesToPress = GetValuesToPressForString(directionKeyPadInFreezing, stringToEnter);
                var length = valuesToPress.Length;
                var stringNumericNumbers = int.Parse(string.Join("", stringToEnter.Select(c => int.TryParse(c.ToString(), out var parsed) ? parsed : -1).Where(q => q >= 0)));
                sum += length * stringNumericNumbers;
            }
            return sum.ToString();
        }

        private static string GetValuesToPressForString(IKeyPad directionKeyPadInFreezing, string stringToEnter)
        {
            var valuesToPress = new List<char>();
            foreach (var charToPress in stringToEnter)
            {
                var neededMoves = directionKeyPadInFreezing.GetMovesNeededToMoveAndPressValue(charToPress);
                valuesToPress.AddRange(neededMoves);
            }
            var valuesToPressString = new string(valuesToPress.ToArray());
            return valuesToPressString;
        }

        public string RunPartB(string inputData)
        {
            throw new NotImplementedException();
        }
    }

    interface IKeyPad
    {
        char[] GetMovesNeededToMoveAndPressValue(char valueToPress);
    }

    class NumericKeyPad : IKeyPad
    {

        private Point CurrentPosition { get; set; }

        private Dictionary<char, Point> ValuePosition { get; set; }

        public NumericKeyPad()
        {
            var keyPadValues = new char[][] {
                ['7','8', '9'],
                ['4', '5', '6'],
                ['1', '2', '3'],
                [' ', '0', 'A']
            };
            ValuePosition = [];
            for (var y = 0; y < keyPadValues.Length; y++)
            {
                for (var x = 0; x < keyPadValues[y].Length; x++)
                {
                    ValuePosition.Add(keyPadValues[y][x], new Point(x, y));
                }
            }
            CurrentPosition = ValuePosition['A'];
        }

        public char[] GetMovesNeededToMoveAndPressValue(char valueToPress)
        {
            var positionToReach = ValuePosition[valueToPress];
            var vector = new Point(positionToReach.X - CurrentPosition.X, positionToReach.Y - CurrentPosition.Y);
            var xMovesSymbol = vector.X > 0 ? '>' : '<';
            var yMovesSymbol = vector.Y > 0 ? 'v' : '^';
            var xMoves = Enumerable.Repeat(xMovesSymbol, Math.Abs(vector.X)).ToArray();
            var yMoves = Enumerable.Repeat(yMovesSymbol, Math.Abs(vector.Y)).ToArray();
            CurrentPosition = positionToReach;
            if (vector.Y > 0)
            {
                return [.. xMoves, .. yMoves, 'A'];
            }
            else
            {
                return [.. yMoves, .. xMoves, 'A'];
            }
        }
    }

    class DirectionalKeyPad : IKeyPad
    {
        private IKeyPad _keyPad { get; set; }
        private Point CurrentPosition { get; set; }
        public Dictionary<char, Point> ValuePosition { get; }

        public DirectionalKeyPad(IKeyPad keyPad)
        {
            _keyPad = keyPad;
            var keyPadValues = new char[][]{
                [' ','^', 'A'],
                ['<', 'v', '>']
            };
            ValuePosition = new Dictionary<char, Point>();
            for (var y = 0; y < keyPadValues.Length; y++)
            {
                for (var x = 0; x < keyPadValues[y].Length; x++)
                {
                    ValuePosition.Add(keyPadValues[y][x], new Point(x, y));
                }
            }
            CurrentPosition = ValuePosition['A'];
        }

        public char[] GetMovesNeededToMoveAndPressValue(char valueToPress)
        {
            var movesNeededOnKeyPad = _keyPad.GetMovesNeededToMoveAndPressValue(valueToPress);
            var movesNeeded = new List<char>();
            foreach (var moveNeeded in movesNeededOnKeyPad)
            {
                var movesForMove = GetMovesNeededToPressValue(moveNeeded);
                //var movesForA = GetMovesNeededToPressValue('A');
                movesNeeded.AddRange(movesForMove);
            }
            return movesNeeded.ToArray();
        }


        private char[] GetMovesNeededToPressValue(char valueToPress)
        {
            var positionToReach = ValuePosition[valueToPress];
            var vector = new Point(positionToReach.X - CurrentPosition.X, positionToReach.Y - CurrentPosition.Y);
            var xMovesSymbol = vector.X > 0 ? '>' : '<';
            var yMovesSymbol = vector.Y > 0 ? 'v' : '^';
            var xMoves = Enumerable.Repeat(xMovesSymbol, Math.Abs(vector.X)).ToArray();
            var yMoves = Enumerable.Repeat(yMovesSymbol, Math.Abs(vector.Y)).ToArray();
            CurrentPosition = positionToReach;
            if (vector.Y < 0)
            {
                return [.. xMoves, .. yMoves, 'A'];
            }
            else
            {
                return [.. yMoves, .. xMoves, 'A'];
            }
        }
    }
}