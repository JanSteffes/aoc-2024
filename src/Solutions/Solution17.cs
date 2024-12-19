using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;

namespace aoc_2024.Solutions
{
    public class Solution17 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var program = new Program(inputData);
            program.Run();
            return program.Output;
        }

        public string RunPartB(string inputData)
        {
            // for test: returns 0,3,5,4,3,0
            // meaning:
            // A has to be 0 at the end
            // be % 8 = 3 before
            // be % 8 = 4 before
            // be % 8 = 5 before
            // be % 8 = 3 before
            // be % 8 = 0 before
            // therefore, must be divisable by 8 at the beginning (calc each 8 * x and run program, maybe in parallel till one works?)
            var notFound = true;
            var expectedResult = string.Join(",", new Program(inputData).Instructions);
            var length = new Program(inputData).Instructions.Count;
            var currentNumbers = 0;
            //var number = (long)BigInteger.Pow(8, length - 1);

            //number = long.Parse(number.ToString().Substring(0, number.ToString().Length - 2) + "00");
            //var number = 226000000000000;
            //var originalNumber = number / 10;
            //var lastResult = "";
            var minValue = 233776540702820;
            //var maxValue = minValue + 50000;//long.Parse(string.Join("", Enumerable.Repeat(9, minValue.Digits_String())));
            var runs = 1;
            var random = new Random();
            do
            {
                // unterchiede erst bei +64
                // zählt von links nach rechts hoch
                // wenn also hinten an Stelle x eine 1 steht, muss davor das feld 7 mal gewesen sein
                // kann man das rückwärts mit ^8 berechnen?
                // GILT NUR FÜRST TESTPROGRAMM B
                var outputs = new ConcurrentBag<string>();
                var numbers = new long[100000];
                for (var i = 0; i < numbers.Length; i++)
                {
                    numbers[i] = minValue + i;
                }
                minValue = numbers.Last();

                // für input: muss vermutlich zwischen
                // 220000000000000 - 6201327566340000 - [6,6,6,2,1,6,3,7,1,4,1,6,7,4,5,0] und 
                // 230000000000000 - 6422741423060000 - [6,6,6,0,6,5,6,5,4,7,3,0,5,5,2,0] liegen

                /*
                 * Letzten 3 Ziffern bei octa zahl:
277
240
155
Ergeben erste 3 Ziffern: [2,4,1






Regex für 3,4,1 irgendwo:

(?:\d,){7}(3,4,1),(?:\d,){5}\d

                 */

                Parallel.ForEach(numbers, number =>
                {
                    //var number = random.NextInt64(minValue, maxValue);
                    var program = new Program(inputData);
                    program.A = number;
                    program.Run();
                    var result = program.Output;
                    var resultString = $"{number} - {Convert.ToString(number, 8)} - [{result}]";
                    if (result.Length > expectedResult.Length)
                    {
                        //maxValue = number;
                        resultString += $" ##### {result.Length - expectedResult.Length} too big";
                    }
                    outputs.Add(resultString);
                });
                var newMaxValueOutputs = outputs.Where(o => o.Contains("####")).ToList();
                //if (newMaxValueOutputs.Any())
                //{
                //    var newMaxValue = newMaxValueOutputs.Select(o => long.Parse(o.Split("-").First().Trim())).OrderBy(o => o).FirstOrDefault();
                //    if (newMaxValue != default)
                //    {
                //        maxValue = newMaxValue;
                //    }
                //}
                var validOutputs = outputs.Except(newMaxValueOutputs).ToList();
                foreach (var output in validOutputs)
                {
                    Debug.WriteLine(output);
                }

            } while (notFound);
            //return results.Min().ToString();
            return string.Empty;
        }
    }


    public class Program
    {

        public long A { get; set; }

        public long B { get; set; }

        public long C { get; set; }

        public int OperandIndex { get; set; }

        public List<string> Outputs { get; set; }

        public List<int> Instructions { get; set; }

        public Program(string instructions)
        {
            var parsed = ParseUtils.ParseIntoLines(instructions);
            A = long.Parse(parsed[0].Split(" ").Last());
            B = long.Parse(parsed[1].Split(" ").Last());
            C = long.Parse(parsed[2].Split(" ").Last());
            Instructions = parsed[3].Split(" ").Last().Split(",").Select(c => int.Parse(c.ToString())).ToList();
            Outputs = [];
        }

        public void Run()
        {
            while (OperandIndex < Instructions.Count)
            {
                var currentInstruction = Instructions[OperandIndex];
                var operand = Instructions[OperandIndex + 1];
                switch (currentInstruction)
                {
                    case 0:
                        adv(operand);
                        OperandIndex += 2;
                        break;
                    case 1:
                        bxl(operand);
                        OperandIndex += 2;
                        break;
                    case 2:
                        bst(operand);
                        OperandIndex += 2;
                        break;
                    case 3:
                        jnz(operand);
                        break;
                    case 4:
                        bxc(operand);
                        OperandIndex += 2;
                        break;
                    case 5:
                        @out(operand);
                        OperandIndex += 2;
                        break;
                    case 6:
                        bdv(operand);
                        OperandIndex += 2;
                        break;
                    case 7:
                        cdv(operand);
                        OperandIndex += 2;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }


        public string Output => string.Join(",", Outputs);

        public void adv(int comboOperand)
        {
            A = dv(comboOperand);
        }

        public void bxl(int literalOperand)
        {
            B = literalOperand ^ B;
        }

        public void bst(int comboOperand)
        {
            const uint mask = (1 << 3) - 1; // 0x3FF
            var operandL = GetComboOperandValue(comboOperand);
            operandL = operandL % 8;
            var last3Bits = operandL & mask;
            B = last3Bits;
        }

        public void jnz(int literalOperand)
        {
            if (A == 0)
            {
                OperandIndex += 2;
                return;
            }
            OperandIndex = literalOperand;
        }

        public void bxc(int _)
        {
            B = B ^ C;
        }

        public void @out(int comboOperand)
        {
            var operandL = GetComboOperandValue(comboOperand);
            var result = operandL % 8;
            Outputs.Add(result.ToString());
        }

        public void bdv(int comboOperand)
        {
            B = dv(comboOperand);
        }

        public void cdv(int comboOperand)
        {
            C = dv(comboOperand);
        }

        private long dv(int comboOperand)
        {
            var operandL = GetComboOperandValue(comboOperand);
            operandL = (long)BigInteger.Pow(2, (int)operandL);
            return A / operandL;
        }



        private long GetComboOperandValue(int operand)
        {
            return operand switch
            {
                4 => A,
                5 => B,
                6 => C,
                _ => operand,
            };
        }
    }
}