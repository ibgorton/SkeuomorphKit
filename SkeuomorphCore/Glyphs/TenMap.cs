using System.Collections.Generic;

namespace SkeuomorphCore;

// Canonical 10-segment layout follows the common A..J naming used by alphanumeric LED references:
// A, B, C, D, E, F, G, H, I, J. J is the additional center segment beyond the standard 9-seg family.
// Reference: https://7seg.fandom.com/wiki/10-segment_display
public abstract class TenMap : SegmentMapBase
{
    public const int SegmentCount = 10;
    private static readonly HashSet<char> SevenSegmentCharacters = [.. SevenMap.SupportedCharacters];

    public const ushort SegmentA = 0x0001;
    public const ushort SegmentB = 0x0002;
    public const ushort SegmentC = 0x0004;
    public const ushort SegmentD = 0x0008;
    public const ushort SegmentE = 0x0010;
    public const ushort SegmentF = 0x0020;
    public const ushort SegmentG = 0x0040;
    public const ushort SegmentH = 0x0080;
    public const ushort SegmentI = 0x0100;
    public const ushort SegmentJ = 0x0200;

    public static readonly string[] SegmentLetters = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J"];
    public static HashSet<char> DisabledCharacters = [];

    private static ushort Mask(params ushort[] segments)
    {
        ushort result = 0;
        foreach (var segment in segments)
        {
            result |= segment;
        }

        return result;
    }

    private static readonly Dictionary<char, ushort> TenMasks = new()
    {
        [' '] = 0x000,
        ['"'] = Mask(SegmentF, SegmentI),
        ['\''] = Mask(SegmentF),
        ['-'] = Mask(SegmentG, SegmentH),
        ['='] = Mask(SegmentD, SegmentG, SegmentH),
        ['_'] = Mask(SegmentD),
        ['<'] = Mask(SegmentD, SegmentE, SegmentG, SegmentH),
        ['0'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentI, SegmentJ),
        ['1'] = Mask(SegmentB, SegmentC, SegmentH),
        ['2'] = Mask(SegmentA, SegmentB, SegmentD, SegmentI),
        ['3'] = Mask(SegmentA, SegmentG, SegmentH, SegmentI),
        ['4'] = Mask(SegmentB, SegmentC, SegmentG, SegmentH),
        ['5'] = Mask(SegmentA, SegmentF, SegmentG, SegmentI),
        ['6'] = Mask(SegmentC, SegmentD, SegmentE, SegmentG, SegmentH),
        ['7'] = Mask(SegmentA, SegmentE, SegmentH),
        ['8'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentG),
        ['9'] = Mask(SegmentA, SegmentB, SegmentF, SegmentG, SegmentI),
        ['$'] = Mask(SegmentA, SegmentC, SegmentD, SegmentF, SegmentG, SegmentH, SegmentI, SegmentJ),
        ['A'] = Mask(SegmentA, SegmentB, SegmentC, SegmentE, SegmentF, SegmentG, SegmentH),
        ['B'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentH, SegmentI, SegmentJ),
        ['C'] = Mask(SegmentA, SegmentD, SegmentE, SegmentF),
        ['D'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentI, SegmentJ),
        ['E'] = Mask(SegmentA, SegmentD, SegmentE, SegmentF, SegmentG),
        ['F'] = Mask(SegmentA, SegmentE, SegmentF, SegmentG),
        ['G'] = Mask(SegmentA, SegmentC, SegmentD, SegmentE, SegmentF, SegmentH),
        ['H'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentG, SegmentH),
        ['I'] = Mask(SegmentA, SegmentD, SegmentI, SegmentJ),
        ['J'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE),
        ['K'] = Mask(SegmentC, SegmentE, SegmentF, SegmentG, SegmentH, SegmentI),
        ['L'] = Mask(SegmentD, SegmentE, SegmentF),
        ['M'] = Mask(SegmentA, SegmentB, SegmentC, SegmentE, SegmentF, SegmentI, SegmentJ),
        ['N'] = Mask(SegmentC, SegmentE, SegmentG, SegmentH),
        ['O'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['P'] = Mask(SegmentA, SegmentB, SegmentE, SegmentF, SegmentG, SegmentH),
        ['Q'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentJ),
        ['R'] = Mask(SegmentA, SegmentB, SegmentE, SegmentF, SegmentG, SegmentH, SegmentJ),
        ['S'] = Mask(SegmentA, SegmentC, SegmentD, SegmentF, SegmentG, SegmentH),
        ['T'] = Mask(SegmentA, SegmentI, SegmentJ),
        ['U'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['V'] = Mask(SegmentB, SegmentC, SegmentF, SegmentG, SegmentJ),
        ['W'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentI, SegmentJ),
        ['X'] = Mask(SegmentB, SegmentE, SegmentG, SegmentH, SegmentI, SegmentJ),
        ['Y'] = Mask(SegmentB, SegmentF, SegmentG, SegmentH, SegmentJ),
        ['Z'] = Mask(SegmentA, SegmentB, SegmentD, SegmentE, SegmentG, SegmentH),
        ['a'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentG, SegmentH),
        ['b'] = Mask(SegmentC, SegmentD, SegmentE, SegmentF, SegmentG, SegmentH),
        ['c'] = Mask(SegmentD, SegmentE, SegmentG, SegmentH),
        ['d'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentG, SegmentH),
        ['e'] = Mask(SegmentA, SegmentB, SegmentD, SegmentE, SegmentF, SegmentG, SegmentH),
        ['f'] = Mask(SegmentA, SegmentE, SegmentF, SegmentG, SegmentH),
        ['g'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentF, SegmentG, SegmentH),
        ['h'] = Mask(SegmentC, SegmentE, SegmentF, SegmentG, SegmentH),
        ['i'] = Mask(SegmentD, SegmentJ),
        ['j'] = Mask(SegmentB, SegmentC, SegmentD),
        ['k'] = Mask(SegmentE, SegmentF, SegmentG, SegmentH, SegmentJ),
        ['l'] = Mask(SegmentD, SegmentE, SegmentF),
        ['n'] = Mask(SegmentC, SegmentE, SegmentG, SegmentH),
        ['o'] = Mask(SegmentC, SegmentD, SegmentE, SegmentG, SegmentH),
        ['r'] = Mask(SegmentE, SegmentG, SegmentH),
        ['s'] = Mask(SegmentA, SegmentC, SegmentD, SegmentF, SegmentG, SegmentH),
        ['t'] = Mask(SegmentD, SegmentE, SegmentF, SegmentG),
        ['u'] = Mask(SegmentC, SegmentD, SegmentE),
        ['v'] = Mask(SegmentC, SegmentD, SegmentJ),
        ['w'] = Mask(SegmentC, SegmentD, SegmentE, SegmentJ),
        ['x'] = Mask(SegmentB, SegmentE, SegmentG, SegmentH, SegmentI, SegmentJ),
        ['y'] = Mask(SegmentB, SegmentC, SegmentD, SegmentF, SegmentG, SegmentH),
        ['z'] = Mask(SegmentA, SegmentB, SegmentD, SegmentE, SegmentG, SegmentH),
        ['!'] = Mask(SegmentI, SegmentJ),
        ['?'] = Mask(SegmentA, SegmentB, SegmentH, SegmentJ),
        ['@'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentG, SegmentH),
        ['´'] = Mask(SegmentH),
        ['`'] = Mask(SegmentH),
        ['|'] = Mask(SegmentE, SegmentF),
        ['m'] = Mask(SegmentC, SegmentE, SegmentG, SegmentH, SegmentJ),
        ['q'] = Mask(SegmentA, SegmentB, SegmentC, SegmentF, SegmentG, SegmentH),
























































    };

    public static IReadOnlyCollection<char> SupportedCharacters => TenMasks.Keys;

    public static bool[] GetBitsTen(char c)
    {
        var original = c;
        var normalized = char.ToUpperInvariant(c);
        if (!TryGetMask(original, normalized, out var mask))
        {
            return new bool[SegmentCount];
        }

        var result = new bool[SegmentCount];
        for (var i = 0; i < SegmentCount; i++)
        {
            result[i] = ((mask >> i) & 1) == 1;
        }

        return result;
    }

    internal static bool TryGetMask(char original, char normalized, out ushort mask)
    {
        if (SevenSegmentCharacters.Contains(original) || SevenSegmentCharacters.Contains(normalized))
        {
            var sevenBits = normalized.GetBitsSeven();
            mask = 0;
            for (var i = 0; i < sevenBits.Length; i++)
            {
                if (sevenBits[i])
                {
                    mask |= (ushort)(1 << i);
                }
            }

            return true;
        }

        if (TenMasks.TryGetValue(original, out mask))
        {
            return true;
        }

        if (TenMasks.TryGetValue(normalized, out mask))
        {
            return true;
        }

        mask = 0;
        return false;
    }
}

public static class TenMapExtensions
{
    public static bool[] GetBitsTen(this char c)
    {
        return TenMap.GetBitsTen(c);
    }
}
