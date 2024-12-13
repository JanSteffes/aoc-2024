using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;
using System.Text.RegularExpressions;

namespace aoc_2024.Solutions
{
    public class Solution13 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var lines = ParseUtils.ParseIntoLines(inputData);
            var sum = 0;
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex += 3)
            {
                var currentLines = lines.Skip(lineIndex).Take(3).ToList();
                var buttonA = ClawMaschineButton.FromLine(currentLines[0]);
                var buttonB = ClawMaschineButton.FromLine(currentLines[1]);
                var price = Price.FromLine(currentLines[2]);
                sum += new ClawMaschine(buttonA, buttonB, price).CalculateMinimumNeededTokens();
            }
            return sum.ToString();
        }

        public string RunPartB(string inputData)
        {
            throw new NotImplementedException();
        }
    }

    class ClawMaschine
    {
        /*
         * A = (p_x*b_y - p_y*b_x) / (a_x*b_y - a_y*b_x)
         * B = (a_x*p_y - a_y*p_x) / (a_x*b_y - a_y*b_x)
         */

        public ClawMaschineButton ButtonA { get; set; }

        public ClawMaschineButton ButtonB { get; set; }

        public Price Price { get; set; }

        public ClawMaschine(ClawMaschineButton buttonA, ClawMaschineButton buttonB, Price price)
        {
            ButtonA = buttonA;
            ButtonB = buttonB;
            Price = price;
        }

        public int CalculateMinimumNeededTokens()
        {
            var a = ((Price.X * ButtonB.IncrementY) - (Price.Y * ButtonB.IncrementX)) / ((ButtonA.IncrementX * ButtonB.IncrementY) - (ButtonA.IncrementY * ButtonB.IncrementX));
            var b = ((ButtonA.IncrementX * Price.Y) - (ButtonA.IncrementY * Price.X)) / ((ButtonA.IncrementX * ButtonB.IncrementY) - (ButtonA.IncrementY * ButtonB.IncrementX));
            return (a * ButtonA.Cost + b * ButtonB.Cost);
        }
    }

    class ClawMaschineButton
    {
        //private const string IncrementRegex = "(?:X\\+)(<IncrementX>\\d+).*(?:Y\\+)(<IncrementY>\\d+)";
        private const string IncrementRegex = "(?:X\\+)(?<X>\\d+).*(?:Y\\+)(?<Y>\\d+)";

        public int IncrementX { get; private set; }

        public int IncrementY { get; private set; }

        public int Cost { get; private set; }
        public ClawMaschineButton(int incrementX, int incrementY, int cost)
        {
            IncrementX = incrementX;
            IncrementY = incrementY;
            Cost = cost;
        }

        public static ClawMaschineButton FromLine(string line)
        {
            var cost = line.Contains("A:") ? 1 : 3;
            var (incrementX, incrementY) = GetIncrements(line, IncrementRegex);
            return new ClawMaschineButton(incrementX, incrementY, cost);
        }

        private static (int x, int y) GetIncrements(string line, string regexToUse)
        {
            var regexToParse = new Regex(regexToUse);
            var regexResult = regexToParse.Match(line);
            var x = int.Parse(regexResult.Groups["X"].Value);
            var y = int.Parse(regexResult.Groups["Y"].Value);
            return (x, y);
        }
    }

    class Price
    {
        private const string PriceCoordinatesRegex = "(?:X\\=)(<X>\\d+).*(?:Y\\=)(<Y>\\d+)";

        public int X { get; set; }

        public int Y { get; set; }

        public Price(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Price FromLine(string line)
        {
            var regexToParse = new Regex(PriceCoordinatesRegex);
            var regexResult = regexToParse.Match(line);
            var x = int.Parse(regexResult.Groups["IncrementX"].Value);
            var y = int.Parse(regexResult.Groups["IncrementY"].Value);
            return new Price(x, y);
        }
    }


}