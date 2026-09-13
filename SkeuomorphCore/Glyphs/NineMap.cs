using System.Collections.Generic;

namespace SkeuomorphCore;

// Canonical 9-segment layout follows the common A..I naming used by real alphanumeric LED parts:
// A, B, C, D, E, F, G, H, I. H and I are the two extra segments beyond the seven-segment core.
// Reference: https://7seg.fandom.com/wiki/7-segment_display and the canonical 9-seg template naming used by the SVG reference.
// This is not a 16-segment subset; it is a distinct display family with its own bit ordering.
public static class NineMap
{
    private const int SegmentCount = 9;
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

    public static readonly string[] SegmentLetters = ["A", "B", "C", "D", "E", "F", "G", "H", "I"];
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

    private static readonly Dictionary<char, ushort> NineMasks = new()
    {
        [' '] = 0x000,
        ['"'] = Mask(SegmentB, SegmentF),
        ['\''] = Mask(SegmentF),
        ['-'] = Mask(SegmentG),
        ['='] = Mask(SegmentD, SegmentG),
        ['_'] = Mask(SegmentD),
        ['<'] = Mask(SegmentH, SegmentI),
        ['0'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['1'] = Mask(SegmentB, SegmentC, SegmentH),
        ['2'] = Mask(SegmentA, SegmentB, SegmentD, SegmentI),
        ['3'] = Mask(SegmentA, SegmentG, SegmentH, SegmentI),
        ['4'] = Mask(SegmentB, SegmentC, SegmentG, SegmentH),
        ['5'] = Mask(SegmentA, SegmentF, SegmentG, SegmentI),
        ['6'] = Mask(SegmentC, SegmentD, SegmentE, SegmentG, SegmentH),
        ['7'] = Mask(SegmentA, SegmentE, SegmentH),
        ['8'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentG),
        ['9'] = Mask(SegmentA, SegmentB, SegmentF, SegmentG, SegmentI),
        ['$'] = Mask(SegmentA, SegmentC, SegmentD, SegmentF, SegmentG),
        ['A'] = Mask(SegmentA, SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['B'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentG),
        ['C'] = Mask(SegmentA, SegmentD, SegmentE, SegmentF),
        ['D'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['E'] = Mask(SegmentA, SegmentD, SegmentE, SegmentF, SegmentG),
        ['F'] = Mask(SegmentA, SegmentE, SegmentF, SegmentG),
        ['G'] = Mask(SegmentA, SegmentC, SegmentD, SegmentE, SegmentF),
        ['H'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['I'] = Mask(SegmentB, SegmentC),
        ['J'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE),
        ['K'] = Mask(SegmentE, SegmentF, SegmentH, SegmentI),
        ['L'] = Mask(SegmentD, SegmentE, SegmentF),
        ['M'] = Mask(SegmentA, SegmentC, SegmentE, SegmentG, SegmentH, SegmentI),
        ['N'] = Mask(SegmentB, SegmentC, SegmentE, SegmentG, SegmentH, SegmentI),
        ['O'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['P'] = Mask(SegmentA, SegmentB, SegmentE, SegmentF, SegmentG, SegmentI),
        ['Q'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentG),
        ['R'] = Mask(SegmentA, SegmentB, SegmentE, SegmentF, SegmentG, SegmentI),
        ['S'] = Mask(SegmentA, SegmentC, SegmentD, SegmentF, SegmentG),
        ['T'] = Mask(SegmentA, SegmentB),
        ['U'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['V'] = Mask(SegmentB, SegmentE, SegmentF, SegmentI),
        ['W'] = Mask(SegmentB, SegmentC, SegmentE, SegmentG, SegmentH, SegmentI),
        ['X'] = Mask(SegmentB, SegmentC, SegmentE, SegmentG, SegmentH, SegmentI),
        ['Y'] = Mask(SegmentB, SegmentC, SegmentF, SegmentH),
        ['Z'] = Mask(SegmentA, SegmentB, SegmentF, SegmentH),
        ['a'] = Mask(SegmentC, SegmentD, SegmentG, SegmentI),
        ['b'] = Mask(SegmentC, SegmentD, SegmentE, SegmentF, SegmentG),
        ['c'] = Mask(SegmentD, SegmentE, SegmentG),
        ['d'] = Mask(SegmentD, SegmentE, SegmentG, SegmentH, SegmentI),
        ['e'] = Mask(SegmentD, SegmentE, SegmentG, SegmentI),
        ['f'] = Mask(SegmentA, SegmentE, SegmentF, SegmentG),
        ['g'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentF, SegmentG),
        ['h'] = Mask(SegmentC, SegmentE, SegmentF, SegmentG),
        ['i'] = Mask(SegmentH),
        ['j'] = Mask(SegmentG, SegmentH),
        ['k'] = Mask(SegmentE, SegmentF, SegmentH, SegmentI),
        ['l'] = Mask(SegmentD, SegmentE, SegmentF),
        ['n'] = Mask(SegmentC, SegmentE, SegmentG),
        ['o'] = Mask(SegmentC, SegmentD, SegmentE, SegmentG),
        ['r'] = Mask(SegmentE, SegmentG),
        ['s'] = Mask(SegmentA, SegmentC, SegmentD, SegmentF, SegmentG),
        ['t'] = Mask(SegmentD, SegmentE, SegmentF, SegmentG),
        ['u'] = Mask(SegmentC, SegmentD, SegmentE),
        ['v'] = Mask(SegmentE, SegmentI),
        ['w'] = Mask(SegmentB, SegmentC, SegmentE, SegmentG, SegmentH, SegmentI),
        ['x'] = Mask(SegmentB, SegmentC, SegmentE, SegmentG, SegmentH, SegmentI),
        ['y'] = Mask(SegmentB, SegmentC, SegmentF, SegmentH),
        ['z'] = Mask(SegmentD, SegmentG, SegmentI),
        ['!'] = Mask(SegmentH),
        ['?'] = Mask(SegmentE, SegmentG, SegmentH),
        ['@'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentG),
        ['´'] = Mask(SegmentH),
        ['`'] = Mask(SegmentH),
        ['|'] = Mask(SegmentH)
    };

    public static IReadOnlyCollection<char> SupportedCharacters => NineMasks.Keys;

    public static bool[] GetBitsNine(this char c)
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

    private static bool TryGetMask(char original, char normalized, out ushort mask)
    {
        if (NineMasks.TryGetValue(original, out mask))
        {
            return true;
        }

        if (NineMasks.TryGetValue(normalized, out mask))
        {
            return true;
        }

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

        mask = 0;
        return false;
    }
}
