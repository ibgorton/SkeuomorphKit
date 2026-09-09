using System;
using System.Globalization;
using System.Linq;

namespace SkeuomorphCommon
{
    public static class DisplayValueFormatter
    {
        public static char[] GetIntegerDisplayChars(double value)
        {
            var (integerChars, _, negative) = ParseDisplayParts(value);
            if (negative)
                return new[] { '-' }.Concat(integerChars).ToArray();

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
                return (Array.Empty<char>(), Array.Empty<char>(), false);

            bool negative = value < 0;
            double magnitude = Math.Abs(value);
            long integerPart = (long)magnitude;
            double fractionalPart = Math.Round(magnitude - integerPart, 10, MidpointRounding.AwayFromZero);

            string integerText = integerPart.ToString(CultureInfo.InvariantCulture);
            string fractionText = string.Empty;
            if (fractionalPart != 0d)
            {
                fractionText = fractionalPart.ToString(CultureInfo.InvariantCulture);
                if (fractionText.Contains('.'))
                {
                    fractionText = fractionText.Split('.')[1].TrimEnd('0');
                }
            }

            return (integerText.ToCharArray(), fractionText.ToCharArray(), negative);
        }
    }
}
