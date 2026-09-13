using System.Collections.Generic;

namespace SkeuomorphCore;

// A real 14-segment display is a 7-segment core plus the common secondary diagonals/center segments,
// not a generic 16-segment subset. The canonical subset is A,B,C,D,E,F,G,H,J,K,L,M,N,P.
public abstract class FourteenMap : SegmentMapBase
{
    public const int SegmentCount = 14;

    // Canonical 14-segment order for real Lite-On-style hardware:
    // A,B,C,D,E,F,G,H,J,K,L,M,N,P.
    public const ushort SegmentA = 0x0001;
    public const ushort SegmentB = 0x0002;
    public const ushort SegmentC = 0x0004;
    public const ushort SegmentD = 0x0008;
    public const ushort SegmentE = 0x0010;
    public const ushort SegmentF = 0x0020;
    public const ushort SegmentG = 0x0040;
    public const ushort SegmentH = 0x0080;
    public const ushort SegmentJ = 0x0100;
    public const ushort SegmentK = 0x0200;
    public const ushort SegmentL = 0x0400;
    public const ushort SegmentM = 0x0800;
    public const ushort SegmentN = 0x1000;
    public const ushort SegmentP = 0x2000;

    public static readonly string[] SegmentLetters = ["A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P"];
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

    // Common letter mappings for real 14-segment displays. These follow the canonical letters used by
    // the hardware reference and intentionally differ from the generic 16-seg subset mapping.
    private static readonly Dictionary<char, ushort> FourteenMasks = new()
    {
        [' '] = 0x0000,
        ['-'] = Mask(SegmentG),
        ['.'] = Mask(SegmentB),
        [':'] = Mask(SegmentB),
        ['='] = Mask(SegmentD, SegmentG),
        ['_'] = Mask(SegmentD),
        ['0'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentM, SegmentP),
        ['1'] = Mask(SegmentB, SegmentC),
        ['2'] = Mask(SegmentA, SegmentB, SegmentD, SegmentE, SegmentG, SegmentH),
        ['3'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentG, SegmentH),
        ['4'] = Mask(SegmentB, SegmentC, SegmentF, SegmentG, SegmentH),
        ['5'] = Mask(SegmentA, SegmentC, SegmentD, SegmentF, SegmentG, SegmentH),
        ['6'] = Mask(SegmentA, SegmentC, SegmentD, SegmentE, SegmentF, SegmentG, SegmentH),
        ['7'] = Mask(SegmentA, SegmentB, SegmentC),
        ['8'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentG, SegmentH),
        ['9'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentF, SegmentG),
        ['A'] = Mask(SegmentA, SegmentB, SegmentC, SegmentE, SegmentF, SegmentG, SegmentH, SegmentK, SegmentL),
        ['B'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentG, SegmentH, SegmentJ, SegmentK, SegmentL),
        ['C'] = Mask(SegmentA, SegmentD, SegmentE, SegmentF),
        ['D'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentJ, SegmentK),
        ['E'] = Mask(SegmentA, SegmentD, SegmentE, SegmentF, SegmentG, SegmentH),
        ['F'] = Mask(SegmentA, SegmentE, SegmentF, SegmentG, SegmentH),
        ['G'] = Mask(SegmentA, SegmentC, SegmentD, SegmentE, SegmentF, SegmentH, SegmentL),
        ['H'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentG, SegmentH),
        ['I'] = Mask(SegmentB, SegmentC),
        ['J'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE),
        ['K'] = Mask(SegmentE, SegmentF, SegmentG, SegmentK, SegmentL),
        ['L'] = Mask(SegmentD, SegmentE, SegmentF),
        ['M'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentH, SegmentK),
        ['N'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentH, SegmentL),
        ['O'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['P'] = Mask(SegmentA, SegmentB, SegmentE, SegmentF, SegmentG, SegmentH, SegmentK),
        ['Q'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentM),
        ['R'] = Mask(SegmentA, SegmentB, SegmentE, SegmentF, SegmentG, SegmentH, SegmentM, SegmentP),
        ['S'] = Mask(SegmentA, SegmentC, SegmentD, SegmentF, SegmentG),
        ['T'] = Mask(SegmentA, SegmentB),
        ['U'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['V'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['W'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentH, SegmentK),
        ['X'] = Mask(SegmentH, SegmentJ, SegmentL, SegmentN),
        ['Y'] = Mask(SegmentB, SegmentC, SegmentF, SegmentG),
        ['Z'] = Mask(SegmentA, SegmentB, SegmentD, SegmentE, SegmentG),
        ['a'] = Mask(SegmentA, SegmentB, SegmentC, SegmentE, SegmentF, SegmentG, SegmentH, SegmentK, SegmentL),
        ['b'] = Mask(SegmentC, SegmentD, SegmentE, SegmentF, SegmentG),
        ['c'] = Mask(SegmentD, SegmentE, SegmentG),
        ['d'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentG),
        ['e'] = Mask(SegmentA, SegmentD, SegmentE, SegmentF, SegmentG, SegmentH),
        ['f'] = Mask(SegmentA, SegmentE, SegmentF, SegmentG, SegmentH),
        ['g'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentF, SegmentG),
        ['h'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['i'] = Mask(SegmentB),
        ['j'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE),
        ['k'] = Mask(SegmentE, SegmentF, SegmentG, SegmentK, SegmentL),
        ['l'] = Mask(SegmentD, SegmentE, SegmentF),
        ['m'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentH, SegmentK),
        ['n'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentH, SegmentL),
        ['o'] = Mask(SegmentC, SegmentD, SegmentE, SegmentG),
        ['p'] = Mask(SegmentA, SegmentB, SegmentE, SegmentF, SegmentG, SegmentH, SegmentK),
        ['q'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentM),
        ['r'] = Mask(SegmentE, SegmentG),
        ['s'] = Mask(SegmentA, SegmentC, SegmentD, SegmentF, SegmentG),
        ['t'] = Mask(SegmentD, SegmentE, SegmentF, SegmentG),
        ['u'] = Mask(SegmentC, SegmentD, SegmentE),
        ['v'] = Mask(SegmentE, SegmentL),
        ['w'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentH, SegmentK),
        ['x'] = Mask(SegmentH, SegmentJ, SegmentL, SegmentN),
        ['y'] = Mask(SegmentB, SegmentC, SegmentF, SegmentG),
        ['z'] = Mask(SegmentA, SegmentB, SegmentD, SegmentE, SegmentG),
        ['!'] = Mask(SegmentB, SegmentC),
        ['"'] = Mask(SegmentB, SegmentN),
        ['#'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentJ, SegmentL, SegmentN, SegmentP),
        ['$'] = Mask(SegmentA, SegmentB, SegmentD, SegmentE, SegmentF, SegmentH, SegmentJ, SegmentL, SegmentN, SegmentP),
        ['%'] = Mask(SegmentA, SegmentD, SegmentE, SegmentH, SegmentJ, SegmentK, SegmentL, SegmentN, SegmentP),
        ['&'] = Mask(SegmentA, SegmentE, SegmentF, SegmentG, SegmentH, SegmentJ, SegmentM, SegmentP),
        ['\''] = Mask(SegmentJ),
        ['('] = Mask(SegmentA, SegmentB, SegmentE, SegmentF),
        [')'] = Mask(SegmentC, SegmentD, SegmentG, SegmentH),
        ['*'] = Mask(SegmentH, SegmentJ, SegmentK, SegmentL, SegmentM, SegmentN, SegmentP),
        ['+'] = Mask(SegmentJ, SegmentL, SegmentN, SegmentP),
        [','] = Mask(SegmentN),
        ['/'] = Mask(SegmentB, SegmentF),
        ['?'] = Mask(SegmentA, SegmentB, SegmentC, SegmentL, SegmentN),
        ['@'] = Mask(SegmentA, SegmentB, SegmentC, SegmentE, SegmentF, SegmentG, SegmentH, SegmentJ, SegmentL),
        ['['] = Mask(SegmentB, SegmentE, SegmentJ, SegmentN),
        ['\\'] = Mask(SegmentH, SegmentM),
        [']'] = Mask(SegmentA, SegmentF, SegmentJ, SegmentN),
        ['^'] = Mask(SegmentM, SegmentP),
        ['{'] = Mask(SegmentB, SegmentE, SegmentJ, SegmentN, SegmentP),
        ['|'] = Mask(SegmentJ, SegmentN),
        ['}'] = Mask(SegmentA, SegmentF, SegmentJ, SegmentL, SegmentN),
        ['~'] = Mask(SegmentK, SegmentL, SegmentP)







    };

    public static bool[] GetBitsFourteen(char c)
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
        if (FourteenMasks.TryGetValue(original, out mask))
        {
            return true;
        }

        if (FourteenMasks.TryGetValue(normalized, out mask))
        {
            return true;
        }

        // Preserve the older overall support behavior when a character is not explicitly mapped,
        // but do not force the generic 16-seg subset to masquerade as a 14-seg alphabet.
        var sixteen = normalized.GetBitsSixteen();
        mask = 0;
        var mapping = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 15 };
        for (var i = 0; i < SegmentCount; i++)
        {
            if (sixteen[mapping[i]])
            {
                mask |= (ushort)(1 << i);
            }
        }

        return true;
    }
}

public static class FourteenMapExtensions
{
    public static bool[] GetBitsFourteen(this char c)
    {
        return FourteenMap.GetBitsFourteen(c);
    }
}
