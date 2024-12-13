using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;
using System.Text.RegularExpressions;

namespace aoc_2024.Solutions
{
    public class Solution13 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var clawMaschines = GetMaschines(inputData);
            var clawMaschinesWithResults = clawMaschines.Select(c => (Maschine: c, Tokens: c.CalculateMinimumNeededTokens())).ToList();
            return clawMaschinesWithResults.Sum(s => s.Tokens).ToString();
        }

        public string RunPartB(string inputData)
        {
            var incrementValue = 10000000000000;
            var clawMaschines = GetMaschines(inputData);
            foreach (var clawMaschine in clawMaschines)
            {
                clawMaschine.Price = new Price(clawMaschine.Price.X + incrementValue, clawMaschine.Price.Y + incrementValue);
            }
            var clawMaschinesWithResults = clawMaschines.Select(c => (Maschine: c, Tokens: c.CalculateMinimumNeededTokens())).ToList();
            return clawMaschinesWithResults.Sum(s => s.Tokens).ToString();
        }

        private static List<ClawMaschine> GetMaschines(string inputData)
        {
            var lines = ParseUtils.ParseIntoLines(inputData);
            var clawMaschines = new List<ClawMaschine>();
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex += 3)
            {
                var currentLines = lines.Skip(lineIndex).Take(3).ToList();
                var buttonA = ClawMaschineButton.FromLine(currentLines[0], 3);
                var buttonB = ClawMaschineButton.FromLine(currentLines[1], 1);
                var price = Price.FromLine(currentLines[2]);
                clawMaschines.Add(new ClawMaschine(buttonA, buttonB, price));
            }

            return clawMaschines;
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

        public long CalculateMinimumNeededTokens()
        {
            var a = ((Price.X * ButtonB.IncrementY) - (Price.Y * ButtonB.IncrementX)) / ((ButtonA.IncrementX * ButtonB.IncrementY) - (ButtonA.IncrementY * ButtonB.IncrementX));
            var b = ((ButtonA.IncrementX * Price.Y) - (ButtonA.IncrementY * Price.X)) / ((ButtonA.IncrementX * ButtonB.IncrementY) - (ButtonA.IncrementY * ButtonB.IncrementX));
            // check if calculation possible
            if (((a * ButtonA.IncrementX) + (b * ButtonB.IncrementX)) == Price.X &&
                ((a * ButtonA.IncrementY) + (b * ButtonB.IncrementY)) == Price.Y)
            {
                return (a * ButtonA.Cost) + (b * ButtonB.Cost);
            }
            return 0;
        }
    }

    class ClawMaschineButton
    {
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

        public static ClawMaschineButton FromLine(string line, int cost)
        {
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
        private const string PriceCoordinatesRegex = "(?:X\\=)(?<X>\\d+).*(?:Y\\=)(?<Y>\\d+)";

        public long X { get; set; }

        public long Y { get; set; }

        public Price(long x, long y)
        {
            X = x;
            Y = y;
        }

        public static Price FromLine(string line)
        {
            var regexToParse = new Regex(PriceCoordinatesRegex);
            var regexResult = regexToParse.Match(line);
            var x = long.Parse(regexResult.Groups["X"].Value);
            var y = long.Parse(regexResult.Groups["Y"].Value);
            return new Price(x, y);
        }
    }


}