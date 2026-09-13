using System;
using System.Collections.Generic;

namespace SkeuomorphCore;

// Canonical bit ordering follows the dmadison/led-segment-ascii reference set:
// 7-segment: DP-G-F-E-D-C-B-A
// See https://github.com/dmadison/led-segment-ascii and https://7seg.fandom.com/wiki/7-segment_display
// Licensed under the MIT license (Copyright © 2017 David Madison).
public static class SevenMap
{
    private const int SegmentCount = 7;

    // The canonical bit ordering follows the common LED naming used by the 7-segment reference docs:
    // A, B, C, D, E, F, G, DP. The value is stored in the same order as the hardware bitmask.
    public const byte SegmentA = 0x01;
    public const byte SegmentB = 0x02;
    public const byte SegmentC = 0x04;
    public const byte SegmentD = 0x08;
    public const byte SegmentE = 0x10;
    public const byte SegmentF = 0x20;
    public const byte SegmentG = 0x40;
    public const byte SegmentDP = 0x80;

    public static readonly string[] SegmentLetters = ["A", "B", "C", "D", "E", "F", "G", "DP"];
    public static HashSet<char> DisabledCharacters = [];

    private static byte Mask(params byte[] segments)
    {
        byte result = 0;
        foreach (var segment in segments)
        {
            result |= segment;
        }

        return result;
    }

    private static readonly Dictionary<char, byte> SevenMasks = new()
    {
        [' '] = 0x00,
        ['-'] = SegmentG,
        ['.'] = Mask(),
        [':'] = Mask(),
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
        ['K'] = Mask(SegmentD, SegmentE, SegmentF, SegmentG),
        ['L'] = Mask(SegmentD, SegmentE, SegmentF),
        ['M'] = Mask(SegmentA, SegmentC, SegmentE, SegmentG),
        ['N'] = Mask(SegmentC, SegmentE, SegmentG),
        ['O'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['P'] = Mask(SegmentA, SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['Q'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentF, SegmentG),
        ['R'] = Mask(SegmentE, SegmentG),
        ['S'] = Mask(SegmentA, SegmentC, SegmentD, SegmentF, SegmentG),
        ['T'] = Mask(SegmentA, SegmentB, SegmentC),
        ['U'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['V'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['W'] = Mask(SegmentC, SegmentD, SegmentE),
        ['X'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['Y'] = Mask(SegmentB, SegmentC, SegmentD, SegmentF, SegmentG),
        ['Z'] = Mask(SegmentA, SegmentB, SegmentD, SegmentE, SegmentG),
        ['a'] = Mask(SegmentA, SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['b'] = Mask(SegmentC, SegmentD, SegmentE, SegmentF, SegmentG),
        ['c'] = Mask(SegmentA, SegmentD, SegmentE, SegmentF),
        ['d'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentG),
        ['e'] = Mask(SegmentA, SegmentD, SegmentE, SegmentF, SegmentG),
        ['f'] = Mask(SegmentA, SegmentE, SegmentF, SegmentG),
        ['g'] = Mask(SegmentA, SegmentC, SegmentD, SegmentE, SegmentF),
        ['h'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['i'] = Mask(SegmentB, SegmentC),
        ['j'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE),
        ['k'] = Mask(SegmentD, SegmentE, SegmentF, SegmentG),
        ['l'] = Mask(SegmentD, SegmentE, SegmentF),
        ['m'] = Mask(SegmentA, SegmentC, SegmentE, SegmentG),
        ['n'] = Mask(SegmentC, SegmentE, SegmentG),
        ['o'] = Mask(SegmentC, SegmentD, SegmentE, SegmentG),
        ['p'] = Mask(SegmentA, SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['q'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentF, SegmentG),
        ['r'] = Mask(SegmentE, SegmentG),
        ['s'] = Mask(SegmentA, SegmentC, SegmentD, SegmentF, SegmentG),
        ['t'] = Mask(SegmentA, SegmentB, SegmentC),
        ['u'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['v'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['w'] = Mask(SegmentC, SegmentD, SegmentE),
        ['x'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['y'] = Mask(SegmentB, SegmentC, SegmentD, SegmentF, SegmentG),
        ['z'] = Mask(SegmentA, SegmentB, SegmentD, SegmentE, SegmentG),
        ['!'] = Mask(SegmentB, SegmentC),
        ['"'] = Mask(SegmentB, SegmentF),
        ['#'] = Mask(SegmentA, SegmentC, SegmentE, SegmentF, SegmentG),
        ['$'] = Mask(SegmentA, SegmentC, SegmentD, SegmentF, SegmentG),
        ['%'] = Mask(SegmentA, SegmentB, SegmentD, SegmentF, SegmentG),
        ['&'] = Mask(SegmentA, SegmentC, SegmentD, SegmentE, SegmentF, SegmentG),
        ['\''] = Mask(SegmentF),
        ['('] = Mask(SegmentA, SegmentD, SegmentE, SegmentF),
        [')'] = Mask(SegmentB, SegmentC, SegmentD, SegmentG),
        ['*'] = Mask(SegmentA, SegmentC, SegmentE, SegmentF, SegmentG),
        ['+'] = Mask(SegmentD, SegmentG),
        [','] = Mask(),
        ['/'] = Mask(SegmentB, SegmentF),
        ['?'] = Mask(SegmentA, SegmentB, SegmentC, SegmentG),
        ['['] = Mask(SegmentA, SegmentD, SegmentE, SegmentF),
        ['\\'] = Mask(SegmentB, SegmentF),
        [']'] = Mask(SegmentA, SegmentD, SegmentE, SegmentF),
        ['^'] = Mask(SegmentA, SegmentB, SegmentF),
        ['{'] = Mask(SegmentA, SegmentD, SegmentE, SegmentF),
        ['|'] = Mask(SegmentB, SegmentC),
        ['}'] = Mask(SegmentA, SegmentD, SegmentE, SegmentF),
        ['~'] = Mask(SegmentG)
    };

    public static IReadOnlyCollection<char> SupportedCharacters => SevenMasks.Keys;

    public static bool[] GetBitsSeven(this char c)
    {
        var result = new bool[SegmentCount];
        GetBitsSeven(c, result);
        return result;
    }

    public static bool GetBitsSeven(char c, Span<bool> destination)
    {
        if (destination.Length < SegmentCount)
        {
            throw new ArgumentException($"Destination span must hold at least {SegmentCount} values.", nameof(destination));
        }

        var normalized = char.ToUpperInvariant(c);
        if (!TryGetMask(c, normalized, out var mask))
        {
            destination.Clear();
            return false;
        }

        for (var i = 0; i < SegmentCount; i++)
        {
            destination[i] = ((mask >> i) & 1) == 1;
        }

        return true;
    }

    private static bool TryGetMask(char original, char normalized, out byte mask)
    {
        if (SevenMasks.TryGetValue(original, out mask))
        {
            return true;
        }

        return SevenMasks.TryGetValue(normalized, out mask);
    }

    public static void GetBitSeven(this bool[] t, char c)
    {
        if (t is null)
        {
            throw new ArgumentNullException(nameof(t));
        }

        GetBitsSeven(c, t.AsSpan());
    }
}
