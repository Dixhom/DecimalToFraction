using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecimalToFraction
{
    class Program
    {
        static void Main(string[] args)
        {
            //foreach (var a in args) Console.WriteLine("{0}", a);


            double d = 0;
            int min = 0;
            int max = 0;

            if (!AreArgsValid(args))
            {
                Console.WriteLine("The parameters are invalid.");
                Console.WriteLine("");


                while (true)
                {
                    Console.WriteLine("Input a decimal number and minimum and maximum denominators");
                    Console.WriteLine("For example, if you want to approximate 3.1415 with denominators from 2 to 99, input '3.1415 2 99'");
                    Console.WriteLine("If you want to quit, just type 'q'");
                    string s = Console.ReadLine();

                    if (s == "q") return;

                    args = s.Split(' ');
                    if (AreArgsValid(args))
                    {
                        break;
                    }

                    Console.WriteLine("The parameters are invalid.");
                    Console.WriteLine("");
                }
            }

            d = double.Parse(args[0]);
            min = int.Parse(args[1]);
            max = int.Parse(args[2]);

            var fs = DecimalToFraction.ConvertDecimalToFraction(d, min, max);

            Console.WriteLine("[approximated fractions of {0}]", d);
            Console.WriteLine("(In order of denominator)");
            Console.WriteLine("---------------------------------------------------");

            foreach (var f in fs)
            {
                Console.WriteLine($"{f.Numerator}/{f.Denominator}\t(error : {(f.Error*100).ToString("G3")}%)");
            }
            Console.WriteLine("");
            Console.WriteLine("[The closest fraction]");
            var closest = DecimalToFraction.GetClosestFraction(fs);
            if(closest != null)
            {
                Console.WriteLine($"{closest.Numerator}/{closest.Denominator}\t(error : {(closest.Error * 100).ToString("G3")}%)");
            }
            else
            {
                Console.WriteLine("none");

            }
            Console.WriteLine("");

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
            return;
        }

        static bool AreArgsValid(string[] args)
        {
            double d = 0;
            int min = 0;
            int max = 0;

            if (args.Length != 3)
            {
                Console.WriteLine("The input should have 3 parameters separated with commas.");
                Console.WriteLine("");
                return false;
            }

            if (!double.TryParse(args[0], out d))
            {
                Console.WriteLine("The first parameter ({0}) is not a number.", args[0]);
                Console.WriteLine("");
                return false;
            }

            if (!int.TryParse(args[1], out min) | !int.TryParse(args[2], out max))
            {
                Console.WriteLine("The min ({0}) or max ({1}) in the input is not an integer.", args[1], args[2]);
                Console.WriteLine("");
                return false;
            }

            return true;
        }


        /// <summary>
        /// Demo of this program. Only for developers seeing this code.
        /// </summary>
        static void Demo()
        {
            // if Euler's number e=2.71828... can be approximated by fractions like X1/2, X2/3, ... , X98/99, 
            // where X1, ... ,X98 are integers, which one will be closest to e?
            // Let's check it.
            var fs = DecimalToFraction.ConvertDecimalToFraction(Math.E, 2, 99);

            Console.WriteLine("*** Euler's number e = {0}", Math.E);
            Console.WriteLine("[In order of denominator]");
            Console.WriteLine("---------------------------------------------------");
            foreach (var f in fs) Console.WriteLine(f.ToString());
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("[The closest fraction]");
            Console.WriteLine(DecimalToFraction.GetClosestFraction(fs).ToString());
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("[In order of the absolute value of error]");
            Console.WriteLine("---------------------------------------------------");
            foreach (var f in fs.OrderBy(f => Math.Abs(f.Error))) Console.WriteLine(f.ToString());
        }

        class DecimalToFraction
        {
            public double DecimalNumber { get; set; }
            public int Denominator { get; set; }
            public int Numerator { get; set; }
            public double Error { get; set; }

            /// <summary>
            /// Get a list of fractions close to a given decimal number for denominators in the given range
            /// </summary>
            /// <param name="decim">a decimal number</param>
            /// <param name="denomRangeMin">the min of the range</param>
            /// <param name="denomRangeMax">the max of the range</param>
            /// <returns>a list of fractions close to a given decimal number</returns>
            static public List<DecimalToFraction> ConvertDecimalToFraction(double decim, int denomRangeMin, int denomRangeMax)
            {
                if (denomRangeMin < 1 || denomRangeMax < 1)
                    throw new ArgumentException("Either rangeMin or rangeMax is less than 0");
                if (denomRangeMin > denomRangeMax)
                    throw new ArgumentException("rangeMin is greater than rangeMax");

                int count = denomRangeMax - denomRangeMin + 1;
                return Enumerable.Range(denomRangeMin, count)
                    .Select(i => DoFractionApproximationAndReducion(decim, i))
                    .Where(f => !IsPowersOf10(f.Denominator)) // it doesn't make sense to have fractions like 3/1, 31/10, 314/100...)
                    .GroupBy(f => f.Denominator) // get distinct elements. (e.g. 3/2, 6/4, 9/6 are all reduced to 3/2)
                    .Select(g => g.First())
                    .ToList();
            }

            /// <summary>
            /// Get the closest fraction to a given decimal number whose denominator is in the given range
            /// </summary>
            /// <param name="decim">a decimal number</param>
            /// <param name="denomRangeMin">the min of the range</param>
            /// <param name="denomRangeMax">the max of the range</param>
            /// <returns>The closest fraction to a given decimal number</returns>
            static public DecimalToFraction GetClosestFraction(double decim, int denomRangeMin, int denomRangeMax)
            {
                var fs = DecimalToFraction.ConvertDecimalToFraction(decim, denomRangeMin, denomRangeMax);
                return DecimalToFraction.GetClosestFraction(fs);
            }

            /// <summary>
            /// Get the closest fraction to a given decimal number among the given fractions
            /// </summary>
            /// <param name="fractions">fractions</param>
            /// <returns>The closest fraction to a given decimal</returns>
            static public DecimalToFraction GetClosestFraction(List<DecimalToFraction> fractions)
            {
                return fractions.OrderBy(f => Math.Abs(f.Error)).First();
            }

            /// <summary>
            /// Approximate a given decimal number to a fraction and reduce it.
            /// </summary>
            /// <param name="decim">decimal number</param>
            /// <param name="denominator">denominator of the fraction of approximation</param>
            /// <returns>the fraction of approximation</returns>
            static private DecimalToFraction DoFractionApproximationAndReducion(double decim, int denominator)
            {
                // fraction approximation
                int numerator = (int)Math.Round(denominator * decim);

                // reduction
                // like 21/9 to 7/3
                int gcd = Gcd(denominator, numerator);
                if (gcd != 1)
                {
                    numerator /= gcd;
                    denominator /= gcd;
                }

                double error = ((double)numerator / denominator - decim) / decim;

                return new DecimalToFraction { DecimalNumber = decim, Numerator = numerator, Denominator = denominator, Error = error };
            }

            /// <summary>
            /// Reduce a fraction (e.g. 21/9 to 7/3)
            /// </summary>
            /// <param name="f"></param>
            /// <returns></returns>
            private DecimalToFraction ReduceFraction(DecimalToFraction f)
            {
                int gcd = Gcd(f.Denominator, f.Numerator);
                if (gcd == 1) return f;
                f.Numerator /= gcd;
                f.Denominator /= gcd;
                return f;
            }

            /// <summary>
            /// Check if the given number is a power of ten (e.g. 1, 10, 100, 1000...)
            /// </summary>
            /// <param name="n">The number to be checked</param>
            /// <returns></returns>
            private bool IsPowerOfTen(int n)
            {
                if (n < 1) return false;
                while (n > 9)
                {
                    if (n % 10 == 0) n /= 10;
                    else return false;
                }
                return n == 1;
            }

            public override string ToString()
            {
                return string.Format("{0} to {1}/{2} (error : {3} %)", DecimalNumber, Numerator, Denominator, Error * 100);
            }

            /// <summary>
            /// Get the greatest common divider of two integers.
            /// </summary>
            /// <param name="n">The first integer</param>
            /// <param name="m">The second integer</param>
            /// <returns>The greatest common divider of two integers</returns>
            private static int Gcd(int n, int m)
            {
                while (n != 0 && m != 0)
                {
                    if (n > m) n %= m;
                    else m %= n;
                }

                return n == 0 ? m : n;
            }

            /// <summary>
            /// Check if a number is a powers of ten (e.g. 1,10,100,1000....) 
            /// </summary>
            /// <param name="n">number to check</param>
            /// <returns>whether a number is a powers of ten</returns>
            static bool IsPowersOf10(int n)
            {
                if (n == 0) return false;

                while (n != 1)
                {
                    if (n % 10 != 0) return false;
                    n /= 10;
                }

                return true;
            }

        }
    }
}
