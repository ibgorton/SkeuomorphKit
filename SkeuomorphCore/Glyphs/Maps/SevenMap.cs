using System.Collections.Generic;

namespace SkeuomorphCore;

// Canonical bit ordering follows the dmadison/led-segment-ascii reference set:
// 7-segment: DP-G-F-E-D-C-B-A
// See https://github.com/dmadison/led-segment-ascii and https://7seg.fandom.com/wiki/7-segment_display
// Licensed under the MIT license (Copyright © 2017 David Madison).
public sealed class SevenMap : SegmentMapBase
{
    public const int SegmentCount = 7;

    public override string MapName => nameof(SevenMap);
    public override int MapSegmentCount => SegmentCount;
    public override IReadOnlyCollection<char> MapSupportedCharacters => SupportedCharacters;

    // The canonical bit ordering follows the common LED naming used by the 7-segment reference docs:    // A, B, C, D, E, F, G, DP. The value is stored in the same order as the hardware bitmask.
    public const byte SegmentA = 0x01;
    public const byte SegmentB = 0x02;
    public const byte SegmentC = 0x04;
    public const byte SegmentD = 0x08;
    public const byte SegmentE = 0x10;
    public const byte SegmentF = 0x20;
    public const byte SegmentG = 0x40;
    public const byte SegmentDP = 0x80;

    private static readonly Dictionary<char, ulong?> DefaultMasks = new()
    {        [' '] = 0x00,
        ['-'] = Mask(SegmentG),
        ['.'] = null,
        [':'] = null,
        ['='] = Mask(SegmentD, SegmentG),
        ['_'] = Mask(SegmentD),
        ['0'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['1'] = Mask(SegmentB, SegmentC),
        ['2'] = Mask(SegmentA, SegmentB, SegmentD, SegmentE, SegmentG),
        ['3'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentG),
        ['4'] = Mask(SegmentB, SegmentC, SegmentF, SegmentG),
        ['5'] = Mask(SegmentA, SegmentC, SegmentD, SegmentF, SegmentG),
        ['6'] = Mask(SegmentA, SegmentC, SegmentD, SegmentE, SegmentF, SegmentG),
        ['7'] = Mask(SegmentA, SegmentB, SegmentC),
        ['8'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentG),
        ['9'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentF, SegmentG),
        ['A'] = Mask(SegmentA, SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['B'] = Mask(SegmentC, SegmentD, SegmentE, SegmentF, SegmentG),
        ['C'] = Mask(SegmentA, SegmentD, SegmentE, SegmentF),
        ['D'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentG),
        ['E'] = Mask(SegmentA, SegmentD, SegmentE, SegmentF, SegmentG),
        ['F'] = Mask(SegmentA, SegmentE, SegmentF, SegmentG),
        ['G'] = Mask(SegmentA, SegmentC, SegmentD, SegmentE, SegmentF),
        ['H'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['I'] = Mask(SegmentB, SegmentC),
        ['J'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE),
        ['K'] = null,
        ['L'] = Mask(SegmentD, SegmentE, SegmentF),
        ['M'] = null,
        ['N'] = Mask(SegmentC, SegmentE, SegmentG),
        ['O'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['P'] = Mask(SegmentA, SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['Q'] = Mask(SegmentA, SegmentB, SegmentC, SegmentF, SegmentG),
        ['R'] = Mask(SegmentE, SegmentG),
        ['S'] = Mask(SegmentA, SegmentC, SegmentD, SegmentF, SegmentG),
        ['T'] = null,
        ['U'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['V'] = null,
        ['W'] = null,
        ['X'] = null,
        ['Y'] = Mask(SegmentB, SegmentC, SegmentD, SegmentF, SegmentG),
        ['Z'] = Mask(SegmentA, SegmentB, SegmentD, SegmentE, SegmentG),
        ['a'] = Mask(SegmentA, SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['b'] = Mask(SegmentC, SegmentD, SegmentE, SegmentF, SegmentG),
        ['c'] = Mask(SegmentD, SegmentE, SegmentG),
        ['d'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentG),
        ['e'] = Mask(SegmentA, SegmentD, SegmentE, SegmentF, SegmentG),
        ['f'] = Mask(SegmentA, SegmentE, SegmentF, SegmentG),
        ['g'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentF, SegmentG),
        ['h'] = Mask(SegmentC, SegmentE, SegmentF, SegmentG),
        ['i'] = Mask(SegmentC),
        ['j'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE),
        ['k'] = Mask(SegmentD, SegmentE, SegmentF, SegmentG),
        ['l'] = Mask(SegmentD, SegmentE, SegmentF),
        ['m'] = Mask(SegmentA, SegmentC, SegmentE, SegmentG),
        ['n'] = Mask(SegmentC, SegmentE, SegmentG),
        ['o'] = Mask(SegmentC, SegmentD, SegmentE, SegmentG),
        ['p'] = Mask(SegmentA, SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['q'] = Mask(SegmentA, SegmentB, SegmentC, SegmentF, SegmentG),
        ['r'] = Mask(SegmentE, SegmentG),
        ['s'] = Mask(SegmentA, SegmentC, SegmentD, SegmentF, SegmentG),
        ['t'] = Mask(SegmentA, SegmentB, SegmentC),
        ['u'] = Mask(SegmentC, SegmentD, SegmentE),
        ['v'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['w'] = Mask(SegmentC, SegmentD, SegmentE),
        ['x'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['y'] = Mask(SegmentB, SegmentC, SegmentD, SegmentF, SegmentG),
        ['z'] = Mask(SegmentA, SegmentB, SegmentD, SegmentE, SegmentG),
        ['!'] = null,
        ['"'] = Mask(SegmentB, SegmentF),
        ['#'] = null,
        ['$'] = null,
        ['%'] = null,
        ['&'] = null,
        ['\''] = Mask(SegmentF),
        ['('] = Mask(SegmentA, SegmentD, SegmentE, SegmentF),
        [')'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD),
        ['*'] = null,
        ['+'] = null,
        [','] = Mask(),
        ['/'] = null,
        ['?'] = Mask(SegmentA, SegmentB, SegmentE, SegmentG),
        ['['] = Mask(SegmentA, SegmentD, SegmentE, SegmentF),
        ['\\'] = null,
        [']'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD),
        ['^'] = Mask(SegmentA, SegmentB, SegmentF),
        ['{'] = null,
        ['|'] = Mask(SegmentE, SegmentF),
        ['}'] = null,
        ['~'] = Mask(SegmentG),
};

    private static readonly Dictionary<char, ulong?> RuntimeMasks = new(DefaultMasks);

    private static byte Mask(params byte[] segments)
    {
        byte result = 0;
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

    public static IReadOnlyCollection<char> SupportedCharacters => new SevenMap().GetSupportedCharacters();
}

