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

    private static byte Mask(params byte[] segments)
    {
        byte result = 0;
        foreach (var segment in segments)
        {
            result |= segment;
        }

        return result;
    }

    // Compact byte-based masks keep the same host-order semantics while eliminating the giant bool[][] tables.
    // Note: this library uses the lower-bit segment ordering A..G and keeps the historical numeric values intact.
    private static readonly Dictionary<char, byte> SevenMasks = new()
    {
        [' '] = 0x00,
        ['-'] = SegmentG,
        ['.'] = SegmentB,
        [':'] = SegmentB,
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
        ['='] = Mask(SegmentD, SegmentG),
        ['+'] = Mask(SegmentD, SegmentG),
        ['/'] = SegmentB,
        ['A'] = Mask(SegmentA, SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['B'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentG),
        ['C'] = Mask(SegmentA, SegmentD, SegmentE, SegmentF),
        ['D'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['E'] = Mask(SegmentA, SegmentD, SegmentE, SegmentF, SegmentG),
        ['F'] = Mask(SegmentA, SegmentE, SegmentF, SegmentG),
        ['G'] = Mask(SegmentA, SegmentC, SegmentD, SegmentE, SegmentF),
        ['H'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['I'] = Mask(SegmentB, SegmentC),
        ['J'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE),
        ['K'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['L'] = Mask(SegmentA, SegmentD, SegmentE, SegmentF),
        ['M'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['N'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['O'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['P'] = Mask(SegmentA, SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['Q'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentG),
        ['R'] = Mask(SegmentA, SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['S'] = Mask(SegmentA, SegmentC, SegmentD, SegmentF, SegmentG),
        ['T'] = SegmentG,
        ['U'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['V'] = Mask(SegmentB, SegmentC, SegmentD, SegmentE, SegmentF),
        ['W'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['X'] = Mask(SegmentB, SegmentC, SegmentE, SegmentF, SegmentG),
        ['Y'] = Mask(SegmentB, SegmentC, SegmentF, SegmentG),
        ['Z'] = Mask(SegmentA, SegmentB, SegmentD, SegmentE, SegmentG),
        ['_'] = SegmentD,
        ['r'] = SegmentE,
        ['o'] = Mask(SegmentC, SegmentD, SegmentE, SegmentG)
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
        if (!DisplayCharacterProfiles.IsSupported("SevenSegment", normalized))
        {
            destination.Clear();
            return false;
        }

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
            throw new ArgumentNullException(nameof(t));

        GetBitsSeven(c, t.AsSpan());
    }
}