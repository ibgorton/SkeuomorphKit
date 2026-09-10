using System;
using System.Globalization;
using System.Linq;

namespace SkeuomorphCore
{
    public static class DisplayValueFormatter
    {
        public static char[] GetIntegerDisplayChars(double value)
        {
            var (integerChars, _, negative) = ParseDisplayParts(value);
            if (negative)
            {
                return new[] { '-' }.Concat(integerChars).ToArray();
            }

            return integerChars;
        }

        public static char[] GetFractionDisplayChars(double value)
        {
            var (_, fractionChars, _) = ParseDisplayParts(value);
            return fractionChars;
        }

        public static (char[] IntegerChars, char[] FractionChars, bool Negative) ParseDisplayParts(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                return (Array.Empty<char>(), Array.Empty<char>(), false);
            }

            bool negative = value < 0d;
            decimal magnitude = Convert.ToDecimal(Math.Abs(value));
            decimal integerPart = decimal.Truncate(magnitude);
            decimal fractionalPart = magnitude - integerPart;

            string integerText = integerPart == 0m ? "0" : integerPart.ToString(CultureInfo.InvariantCulture);
            string fractionText = string.Empty;
            if (fractionalPart != 0m)
            {
                fractionText = fractionalPart.ToString("0.###################", CultureInfo.InvariantCulture);
                int decimalIndex = fractionText.IndexOf('.');
                if (decimalIndex >= 0)
                {
                    fractionText = fractionText[(decimalIndex + 1)..];
                }

                fractionText = fractionText.TrimEnd('0');
            }

            return (integerText.ToCharArray(), fractionText.ToCharArray(), negative);
        }
    }
}
