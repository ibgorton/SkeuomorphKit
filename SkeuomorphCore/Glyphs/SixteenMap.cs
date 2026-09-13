using System.Collections.Generic;

namespace SkeuomorphCore;

// Canonical 16-segment bit ordering follows the dmadison/led-segment-ascii reference set:
// no-decimal-point mask order: DP is handled separately by the display model, so the 16-bit values
// are aligned to the upstream library's NDP table (A..P, with bit 0 = A, bit 15 = P).
// See https://github.com/dmadison/led-segment-ascii
// Licensed under the MIT license (Copyright © 2017 David Madison).
public abstract class SixteenMap : SegmentMapBase
{
    public const int SegmentCount = 16;

    // Canonical 16-seg naming used by the upstream dmadison mask table:
    // A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P.
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
    public const ushort SegmentK = 0x0400;
    public const ushort SegmentL = 0x0800;
    public const ushort SegmentM = 0x1000;
    public const ushort SegmentN = 0x2000;
    public const ushort SegmentO = 0x4000;
    public const ushort SegmentP = 0x8000;

    public static readonly string[] SegmentLetters = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P"];
    public static HashSet<char> DisabledCharacters = ['.', ':'];

    private static ushort Mask(params ushort[] segments)
    {
        ushort result = 0;
        foreach (var segment in segments)
        {
            result |= segment;
        }

        return result;
    }

    // Canonical NDP mask table from dmadison/led-segment-ascii.
    private static readonly Dictionary<char, ushort> SixteenMasks = new()
    {
        [' '] = 0x0000,
        ['!'] = Mask(SegmentJ, SegmentN),
        ['"'] = Mask(SegmentH, SegmentJ),
        ['#'] = Mask(SegmentC, SegmentD, SegmentE, SegmentF, SegmentJ, SegmentL, SegmentN, SegmentP),
        ['$'] = Mask(SegmentA, SegmentB, SegmentD, SegmentE, SegmentF, SegmentH, SegmentJ, SegmentL, SegmentN, SegmentP),
        ['%'] = Mask(SegmentA, SegmentD, SegmentE, SegmentH, SegmentJ, SegmentK, SegmentL, SegmentN, SegmentO, SegmentP),
        ['&'] = Mask(SegmentA, SegmentE, SegmentF, SegmentG, SegmentI, SegmentJ, SegmentM, SegmentP),
        ['\''] = Mask(SegmentH),
        ['('] = Mask(SegmentK, SegmentM),
        [')'] = Mask(SegmentI, SegmentO),
        ['*'] = Mask(SegmentI, SegmentJ, SegmentK, SegmentL, SegmentM, SegmentN, SegmentO, SegmentP),
        ['+'] = Mask(SegmentJ, SegmentL, SegmentN, SegmentP),
        [','] = Mask(SegmentO),
        ['-'] = Mask(SegmentL, SegmentP),
        ['.'] = Mask(),
        ['/'] = Mask(SegmentK, SegmentO),
        ['0'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentG, SegmentH, SegmentK, SegmentO),
        ['1'] = Mask(SegmentA, SegmentE, SegmentF, SegmentJ, SegmentN),
        ['2'] = Mask(SegmentA, SegmentB, SegmentC, SegmentE, SegmentF, SegmentG, SegmentL, SegmentP),
        ['3'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentL),
        ['4'] = Mask(SegmentC, SegmentD, SegmentH, SegmentL, SegmentP),
        ['5'] = Mask(SegmentA, SegmentB, SegmentE, SegmentF, SegmentH, SegmentM, SegmentP),
        ['6'] = Mask(SegmentA, SegmentB, SegmentD, SegmentE, SegmentF, SegmentG, SegmentH, SegmentL, SegmentP),
        ['7'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD),
        ['8'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentG, SegmentH, SegmentL, SegmentP),
        ['9'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentH, SegmentL, SegmentP),
        [':'] = Mask(),
        [';'] = Mask(SegmentA, SegmentO),
        ['<'] = Mask(SegmentK, SegmentM, SegmentP),
        ['='] = Mask(SegmentE, SegmentF, SegmentL, SegmentP),
        ['>'] = Mask(SegmentI, SegmentL, SegmentO),
        ['?'] = Mask(SegmentA, SegmentB, SegmentC, SegmentL, SegmentN),
        ['@'] = Mask(SegmentA, SegmentB, SegmentC, SegmentE, SegmentF, SegmentG, SegmentH, SegmentJ, SegmentL),
        ['A'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentG, SegmentH, SegmentL, SegmentP),
        ['B'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentJ, SegmentL, SegmentN),
        ['C'] = Mask(SegmentA, SegmentB, SegmentE, SegmentF, SegmentG, SegmentH),
        ['D'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentJ, SegmentN),
        ['E'] = Mask(SegmentA, SegmentB, SegmentE, SegmentF, SegmentG, SegmentH, SegmentP),
        ['F'] = Mask(SegmentA, SegmentB, SegmentG, SegmentH, SegmentP),
        ['G'] = Mask(SegmentA, SegmentB, SegmentD, SegmentE, SegmentF, SegmentG, SegmentH, SegmentL),
        ['H'] = Mask(SegmentC, SegmentD, SegmentG, SegmentH, SegmentL, SegmentP),
        ['I'] = Mask(SegmentA, SegmentB, SegmentE, SegmentF, SegmentJ, SegmentN),
        ['J'] = Mask(SegmentC, SegmentD, SegmentE, SegmentF, SegmentG),
        ['K'] = Mask(SegmentG, SegmentH, SegmentK, SegmentM, SegmentP),
        ['L'] = Mask(SegmentE, SegmentF, SegmentG, SegmentH),
        ['M'] = Mask(SegmentC, SegmentD, SegmentG, SegmentH, SegmentI, SegmentK),
        ['N'] = Mask(SegmentC, SegmentD, SegmentG, SegmentH, SegmentI, SegmentM),
        ['O'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentG, SegmentH),
        ['P'] = Mask(SegmentA, SegmentB, SegmentC, SegmentG, SegmentH, SegmentL, SegmentP),
        ['Q'] = Mask(SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentG, SegmentH, SegmentM),
        ['R'] = Mask(SegmentA, SegmentB, SegmentC, SegmentG, SegmentH, SegmentL, SegmentM, SegmentP),
        ['S'] = Mask(SegmentA, SegmentB, SegmentD, SegmentE, SegmentF, SegmentH, SegmentL, SegmentP),
        ['T'] = Mask(SegmentA, SegmentB, SegmentJ, SegmentN),
        ['U'] = Mask(SegmentC, SegmentD, SegmentE, SegmentF, SegmentG, SegmentH),
        ['V'] = Mask(SegmentC, SegmentD, SegmentI, SegmentM),
        ['W'] = Mask(SegmentC, SegmentD, SegmentG, SegmentH, SegmentM, SegmentO),
        ['X'] = Mask(SegmentI, SegmentK, SegmentM, SegmentO),
        ['Y'] = Mask(SegmentC, SegmentH, SegmentL, SegmentN, SegmentP),
        ['Z'] = Mask(SegmentA, SegmentB, SegmentE, SegmentF, SegmentK, SegmentO),
        ['['] = Mask(SegmentB, SegmentE, SegmentJ, SegmentN),
        ['\\'] = Mask(SegmentI, SegmentM),
        [']'] = Mask(SegmentA, SegmentF, SegmentJ, SegmentN),
        ['^'] = Mask(SegmentM, SegmentO),
        ['_'] = Mask(SegmentE, SegmentF),
        ['`'] = Mask(SegmentA),
        ['a'] = Mask(SegmentE, SegmentF, SegmentG, SegmentN, SegmentP),
        ['b'] = Mask(SegmentF, SegmentG, SegmentH, SegmentN, SegmentP),
        ['c'] = Mask(SegmentF, SegmentG, SegmentP),
        ['d'] = Mask(SegmentC, SegmentD, SegmentE, SegmentL, SegmentN),
        ['e'] = Mask(SegmentF, SegmentG, SegmentO, SegmentP),
        ['f'] = Mask(SegmentB, SegmentJ, SegmentL, SegmentN, SegmentP),
        ['g'] = Mask(SegmentA, SegmentF, SegmentH, SegmentJ, SegmentN, SegmentP),
        ['h'] = Mask(SegmentG, SegmentH, SegmentN, SegmentP),
        ['i'] = Mask(SegmentN),
        ['j'] = Mask(SegmentF, SegmentG, SegmentJ, SegmentN),
        ['k'] = Mask(SegmentJ, SegmentK, SegmentM, SegmentN),
        ['l'] = Mask(SegmentF, SegmentG, SegmentH),
        ['m'] = Mask(SegmentD, SegmentG, SegmentL, SegmentN, SegmentP),
        ['n'] = Mask(SegmentG, SegmentN, SegmentP),
        ['o'] = Mask(SegmentF, SegmentG, SegmentN, SegmentP),
        ['p'] = Mask(SegmentA, SegmentG, SegmentH, SegmentJ, SegmentP),
        ['q'] = Mask(SegmentA, SegmentH, SegmentJ, SegmentN, SegmentP),
        ['r'] = Mask(SegmentG, SegmentP),
        ['s'] = Mask(SegmentA, SegmentF, SegmentH, SegmentN, SegmentP),
        ['t'] = Mask(SegmentF, SegmentG, SegmentH, SegmentP),
        ['u'] = Mask(SegmentF, SegmentG, SegmentN),
        ['v'] = Mask(SegmentG, SegmentO),
        ['w'] = Mask(SegmentD, SegmentG, SegmentM, SegmentO),
        ['x'] = Mask(SegmentI, SegmentK, SegmentM, SegmentO),
        ['y'] = Mask(SegmentC, SegmentD, SegmentE, SegmentJ, SegmentL),
        ['z'] = Mask(SegmentF, SegmentO, SegmentP),
        ['{'] = Mask(SegmentB, SegmentE, SegmentJ, SegmentN, SegmentP),
        ['|'] = Mask(SegmentG, SegmentH),
        ['}'] = Mask(SegmentA, SegmentF, SegmentJ, SegmentL, SegmentN),
        ['~'] = Mask(SegmentD, SegmentG, SegmentM, SegmentP),
    };

    public static bool[] GetBitsSixteen(char c)
    {
        var key = char.ToUpperInvariant(c);
        if (!DisplayCharacterProfiles.IsSupported("SixteenSegment", key))
        {
            return new bool[SegmentCount];
        }

        if (TryGetMask(c, key, out var mask))
        {
            var result = new bool[SegmentCount];
            for (var i = 0; i < SegmentCount; i++)
            {
                result[i] = ((mask >> i) & 1) == 1;
            }

            return result;
        }

        return new bool[SegmentCount];
    }

    internal static bool TryGetMask(char original, char normalized, out ushort mask)
    {
        if (SixteenMasks.TryGetValue(original, out mask))
        {
            return true;
        }

        return SixteenMasks.TryGetValue(normalized, out mask);
    }
}

public static class SixteenMapExtensions
{
    public static bool[] GetBitsSixteen(this char c)
    {
        return SixteenMap.GetBitsSixteen(c);
    }
}