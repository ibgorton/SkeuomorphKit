using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SkeuomorphCore;

/// <summary>
/// Formats numeric values into display-friendly character arrays.
/// </summary>
public static class DisplayValueFormatter
{
    /// <summary>
    /// Gets the integer portion of a value as display characters.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <returns>The integer digits, with a leading minus sign when the value is negative.</returns>
    public static char[] GetIntegerDisplayChars(double value)
    {
        var (integerChars, _, negative) = ParseDisplayParts(value);
        if (negative)
        {
            return new[] { '-' }.Concat(integerChars).ToArray();
        }

        return integerChars;
    }

    /// <summary>
    /// Gets the fractional portion of a value as display characters.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <returns>The fractional digits without the decimal point.</returns>
    public static char[] GetFractionDisplayChars(double value)
    {
        var (_, fractionChars, _) = ParseDisplayParts(value);
        return fractionChars;
    }

    /// <summary>
    /// Splits a numeric value into its integer and fractional display parts and sign.
    /// </summary>
    /// <param name="value">The value to parse.</param>
    /// <returns>A tuple containing the integer characters, fractional characters, and whether the value is negative.</returns>
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

    /// <summary>
    /// Produces a single display glyph sequence for a composed string such as a time, date, or decimal value.
    /// Each character is normalized to the active profile's canonical glyph set before being emitted.
    /// </summary>
    public static char[] GetDisplayChars(string value, string profileName)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (string.IsNullOrWhiteSpace(profileName))
        {
            throw new ArgumentException("A display profile name is required.", nameof(profileName));
        }

        var output = new List<char>();
        foreach (var character in value)
        {
            var normalized = NormalizeDisplayCharacter(character);
            if (IsCompositeDisplayToken(normalized))
            {
                output.Add(normalized);
                continue;
            }

            if (!DisplayCharacterProfiles.IsCharacterEnabled(profileName, normalized))
            {
                throw new InvalidOperationException($"Character '{character}' is not supported by profile '{profileName}'.");
            }

            output.Add(normalized);
        }

        return output.ToArray();
    }

    public static bool CanDisplay(string value, string profileName)
    {
        if (string.IsNullOrEmpty(value))
        {
            return true;
        }

        try
        {
            _ = GetDisplayChars(value, profileName);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    private static char NormalizeDisplayCharacter(char character)
    {
        return character switch
        {
            '−' or '–' or '—' or '‒' or '―' => '-',
            '·' or '•' or '∙' or '⋅' => '.',
            '：' or '∶' => ':',
            '＋' => '+',
            '／' => '/',
            '＼' => '\\',
            '％' => '%',
            '°' => '°',
            _ => character
        };
    }

    private static bool IsCompositeDisplayToken(char character)
    {
        return character is '.' or ':' or '-' or '+' or '/' or '\\' or ' ' or '_' or ',' or '=' or '%' or '°';
    }
}
