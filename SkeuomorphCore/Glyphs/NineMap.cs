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

    // Canonical bit order matches the A..I naming in the reference layout:
    // A=bit0, B=bit1, C=bit2, D=bit3, E=bit4, F=bit5, G=bit6, H=bit7, I=bit8.
    // These masks are taken from the canonical 9-segment template used in the reference SVG.
    // Any character that a 7-segment display can render should render identically on the 9-segment
    // family, with the extra H/I segments left blank.
    private static readonly Dictionary<char, ushort> NineMasks = new()
    {
        [' '] = 0x000,
        ['"'] = 0x022,
        ['\''] = 0x020,
        ['-'] = 0x040,
        ['='] = 0x048,
        ['_'] = 0x008,
        ['<'] = 0x180,
        ['0'] = 0x03F,
        ['1'] = 0x086,
        ['2'] = 0x10B,
        ['3'] = 0x1C1,
        ['4'] = 0x0C6,
        ['5'] = 0x161,
        ['6'] = 0x0DC,
        ['7'] = 0x091,
        ['8'] = 0x07F,
        ['9'] = 0x163,
        ['$'] = 0x06D,
        ['A'] = 0x077,
        ['B'] = 0x07F,
        ['C'] = 0x039,
        ['D'] = 0x03E,
        ['E'] = 0x079,
        ['F'] = 0x071,
        ['G'] = 0x03D,
        ['H'] = 0x076,
        ['J'] = 0x01E,
        ['K'] = 0x1B0,
        ['L'] = 0x038,
        ['N'] = 0x1D6,
        ['O'] = 0x03F,
        ['R'] = 0x173,
        ['S'] = 0x06D,
        ['T'] = 0x003,
        ['U'] = 0x03E,
        ['V'] = 0x132,
        ['W'] = 0x1D6,
        ['X'] = 0x1D6,
        ['Y'] = 0x0A6,
        ['Z'] = 0x0A3,
        ['a'] = 0x14C,
        ['b'] = 0x07C,
        ['c'] = 0x058,
        ['d'] = 0x1D8,
        ['e'] = 0x158,
        ['f'] = 0x071,
        ['g'] = 0x06F,
        ['h'] = 0x074,
        ['i'] = 0x080,
        ['j'] = 0x0C0,
        ['k'] = 0x1B0,
        ['l'] = 0x038,
        ['n'] = 0x054,
        ['o'] = 0x05C,
        ['r'] = 0x050,
        ['s'] = 0x06D,
        ['t'] = 0x078,
        ['u'] = 0x01C,
        ['v'] = 0x110,
        ['w'] = 0x1D6,
        ['x'] = 0x1D6,
        ['y'] = 0x0A6,
        ['z'] = 0x148,
        ['!'] = 0x080,
        ['?'] = 0x0D0,
        ['@'] = 0x07F,
        ['\u00B4'] = 0x080,
        ['`'] = 0x080,
        ['|'] = 0x080
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
