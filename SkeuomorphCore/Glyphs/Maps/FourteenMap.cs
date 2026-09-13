using System.Collections.Generic;

namespace SkeuomorphCore;

// A real 14-segment display is a 7-segment core plus the common secondary diagonals/center segments,
// not a generic 16-segment subset. The canonical subset is A,B,C,D,E,F,G,H,J,K,L,M,N,P.
public sealed class FourteenMap : SegmentMapBase
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

    public override string MapName => nameof(FourteenMap);
    public override int MapSegmentCount => SegmentCount;
    public override IReadOnlyCollection<char> MapSupportedCharacters => SupportedCharacters;

    private static readonly Dictionary<char, ulong?> DefaultMasks = new()
    {
        [' '] = 0x0000,
        ['!'] = Mask(SegmentJ, SegmentK),
        ['"'] = Mask(SegmentF, SegmentJ),
        ['#'] = null,
        ['$'] = Mask(SegmentA, SegmentC, SegmentD, SegmentF, SegmentG, SegmentH, SegmentJ, SegmentK),
        ['%'] = null,
        ['&'] = Mask(SegmentA, SegmentD, SegmentE, SegmentG, SegmentL, SegmentM, SegmentN),
        ['\''] = Mask(SegmentJ),
        ['('] = Mask(SegmentA, SegmentD, SegmentE, SegmentF),
        [')'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD),
        ['*'] = Mask(SegmentG, SegmentH, SegmentJ, SegmentK, SegmentL, SegmentM, SegmentN, SegmentP),
        ['+'] = Mask(SegmentG, SegmentH, SegmentJ, SegmentK),
        [','] = Mask(SegmentP),
        ['-'] = Mask(SegmentG, SegmentH),
        ['.'] = null,
        ['/'] = Mask(SegmentM, SegmentP),
        ['0'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentM, SegmentP),
        ['1'] = Mask(SegmentB, SegmentC),
        ['2'] = Mask(SegmentA, SegmentB, SegmentD, SegmentE, SegmentG, SegmentH),
        ['3'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentG, SegmentH),
        ['4'] = Mask(SegmentB, SegmentC, SegmentF, SegmentG, SegmentH),
        ['5'] = Mask(SegmentA, SegmentC, SegmentD, SegmentF, SegmentG, SegmentH),
        ['6'] = Mask(SegmentA, SegmentC, SegmentD, SegmentE, SegmentF, SegmentG, SegmentH),
        ['7'] = Mask(SegmentA, SegmentB, SegmentC),
        ['8'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentG, SegmentH),
        ['9'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentF, SegmentG, SegmentH),
        [':'] = null,
        [';'] = Mask(SegmentJ, SegmentP),
        ['<'] = Mask(SegmentM, SegmentN),
        ['='] = Mask(SegmentD, SegmentG, SegmentH),
        ['>'] = Mask(SegmentL, SegmentP),
        ['?'] = Mask(SegmentA, SegmentB, SegmentH, SegmentK),
        ['@'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentG, SegmentH),
        ['A'] = Mask(SegmentA, SegmentB, SegmentC, SegmentE, SegmentF, SegmentG, SegmentH, SegmentK, SegmentL),
        ['B'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentH, SegmentJ, SegmentK),
        ['C'] = Mask(SegmentA, SegmentD, SegmentE, SegmentF),
        ['D'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentJ, SegmentK),
        ['E'] = Mask(SegmentA, SegmentD, SegmentE, SegmentF, SegmentG, SegmentH),
        ['F'] = Mask(SegmentA, SegmentE, SegmentF, SegmentG, SegmentH),
        ['G'] = Mask(SegmentA, SegmentC, SegmentD, SegmentE, SegmentF, SegmentH),
        ['H'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentG, SegmentH),
        ['I'] = Mask(SegmentA, SegmentD, SegmentJ, SegmentK),
        ['J'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE),
        ['K'] = Mask(SegmentC, SegmentE, SegmentF, SegmentG, SegmentH, SegmentM),
        ['L'] = Mask(SegmentD, SegmentE, SegmentF),
        ['M'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentH, SegmentK),
        ['N'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentL, SegmentN),
        ['O'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['P'] = Mask(SegmentA, SegmentB, SegmentE, SegmentF, SegmentG, SegmentH),
        ['Q'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentN),
        ['R'] = Mask(SegmentA, SegmentB, SegmentE, SegmentF, SegmentG, SegmentH, SegmentN),
        ['S'] = Mask(SegmentA, SegmentC, SegmentD, SegmentF, SegmentG, SegmentH),
        ['T'] = Mask(SegmentA, SegmentJ, SegmentK),
        ['U'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['V'] = Mask(SegmentK, SegmentL, SegmentM),
        ['W'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentJ, SegmentK),
        ['X'] = Mask(SegmentL, SegmentM, SegmentN, SegmentP),
        ['Y'] = Mask(SegmentB, SegmentF, SegmentG, SegmentH, SegmentK),
        ['Z'] = Mask(SegmentA, SegmentD, SegmentM, SegmentP),
        ['['] = Mask(SegmentA, SegmentD, SegmentE, SegmentF),
        ['\\'] = Mask(SegmentL, SegmentN),
        [']'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD),
        ['^'] = Mask(SegmentN, SegmentP),
        ['_'] = Mask(SegmentD),
        ['a'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentG, SegmentH),
        ['b'] = Mask(SegmentC, SegmentD, SegmentE, SegmentF, SegmentG, SegmentH),
        ['c'] = Mask(SegmentD, SegmentE, SegmentG, SegmentH),
        ['d'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentG, SegmentH),
        ['e'] = Mask(SegmentA, SegmentB, SegmentD, SegmentE, SegmentF, SegmentG, SegmentH),
        ['f'] = Mask(SegmentA, SegmentE, SegmentF, SegmentG, SegmentH),
        ['g'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentF, SegmentG, SegmentH),
        ['h'] = Mask(SegmentE, SegmentF, SegmentG, SegmentK),
        ['i'] = Mask(SegmentK),
        ['j'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE),
        ['k'] = Mask(SegmentE, SegmentF, SegmentG, SegmentH, SegmentN),
        ['l'] = Mask(SegmentD, SegmentE, SegmentF),
        ['m'] = Mask(SegmentC, SegmentE, SegmentG, SegmentH, SegmentK),
        ['n'] = Mask(SegmentE, SegmentG, SegmentN),
        ['o'] = Mask(SegmentC, SegmentD, SegmentE, SegmentG, SegmentH),
        ['p'] = Mask(SegmentA, SegmentB, SegmentE, SegmentF, SegmentG, SegmentH),
        ['q'] = Mask(SegmentA, SegmentB, SegmentC, SegmentF, SegmentG, SegmentH),
        ['r'] = Mask(SegmentE, SegmentG, SegmentH),
        ['s'] = Mask(SegmentA, SegmentC, SegmentD, SegmentF, SegmentG, SegmentH),
        ['t'] = Mask(SegmentG, SegmentH, SegmentJ, SegmentK),
        ['u'] = Mask(SegmentC, SegmentD, SegmentE),
        ['v'] = Mask(SegmentE, SegmentP),
        ['w'] = Mask(SegmentC, SegmentD, SegmentE, SegmentK),
        ['x'] = Mask(SegmentL, SegmentM, SegmentN, SegmentP),
        ['y'] = Mask(SegmentB, SegmentC, SegmentD, SegmentF, SegmentG, SegmentH),
        ['z'] = Mask(SegmentA, SegmentD, SegmentM, SegmentP),
        ['{'] = Mask(SegmentG, SegmentM, SegmentN),
        ['|'] = Mask(SegmentE, SegmentF),
        ['}'] = Mask(SegmentA, SegmentF, SegmentJ, SegmentL, SegmentN),
        ['~'] = null,
    };

    private static readonly Dictionary<char, ulong?> RuntimeMasks = new(DefaultMasks);

    private static ushort Mask(params ushort[] segments)
    {
        ushort result = 0;
        foreach (var segment in segments)
        {
            result |= segment;
        }

        return result;
    }

    public override IReadOnlyDictionary<char, ulong?> Masks => RuntimeMasks;

    protected override IReadOnlyDictionary<char, ulong?> GetDefaultMasks()
    {
        return DefaultMasks;
    }

    public static IReadOnlyCollection<char> SupportedCharacters => new FourteenMap().GetSupportedCharacters();
}

