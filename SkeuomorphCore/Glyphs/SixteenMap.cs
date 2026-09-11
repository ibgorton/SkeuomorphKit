using System;
using System.Collections;
using System.Collections.Generic;

namespace SkeuomorphCore
{
    // Canonical 16-segment bit ordering follows the dmadison/led-segment-ascii reference set:
    // no-decimal-point mask order: DP is handled separately by the display model, so the 16-bit values
    // are aligned to the upstream library's NDP table (A..P, with bit 0 = A, bit 15 = P).
    // See https://github.com/dmadison/led-segment-ascii
    // Licensed under the MIT license (Copyright © 2017 David Madison).
    public static class SixteenMap
    {
        private const int SegmentCount = 16;

        // Canonical NDP mask table from dmadison/led-segment-ascii.
        private static readonly Dictionary<char, ushort> SixteenMasks = new()
        {
            [' '] = 0x0000,
            ['!'] = 0x000C,
            ['"'] = 0x0204,
            ['#'] = 0xAA3C,
            ['$'] = 0xAABB,
            ['%'] = 0xEE99,
            ['&'] = 0x9371,
            ['\''] = 0x0200,
            ['('] = 0x1400,
            [')'] = 0x4100,
            ['*'] = 0xFF00,
            ['+'] = 0xAA00,
            [','] = 0x4000,
            ['-'] = 0x8800,
            ['.'] = 0x1000,
            ['/'] = 0x4400,
            ['0'] = 0x44FF,
            ['1'] = 0x040C,
            ['2'] = 0x8877,
            ['3'] = 0x083F,
            ['4'] = 0x888C,
            ['5'] = 0x90B3,
            ['6'] = 0x88FB,
            ['7'] = 0x000F,
            ['8'] = 0x88FF,
            ['9'] = 0x88BF,
            [':'] = 0x2200,
            [';'] = 0x4200,
            ['<'] = 0x9400,
            ['='] = 0x8830,
            ['>'] = 0x4900,
            ['?'] = 0x2807,
            ['@'] = 0x0AF7,
            ['A'] = 0x88CF,
            ['B'] = 0x2A3F,
            ['C'] = 0x00F3,
            ['D'] = 0x223F,
            ['E'] = 0x80F3,
            ['F'] = 0x80C3,
            ['G'] = 0x08FB,
            ['H'] = 0x88CC,
            ['I'] = 0x2233,
            ['J'] = 0x007C,
            ['K'] = 0x94C0,
            ['L'] = 0x00F0,
            ['M'] = 0x05CC,
            ['N'] = 0x11CC,
            ['O'] = 0x00FF,
            ['P'] = 0x88C7,
            ['Q'] = 0x10FF,
            ['R'] = 0x98C7,
            ['S'] = 0x88BB,
            ['T'] = 0x2203,
            ['U'] = 0x00FC,
            ['V'] = 0x44C0,
            ['W'] = 0x50CC,
            ['X'] = 0x5500,
            ['Y'] = 0x88BC,
            ['Z'] = 0x4433,
            ['['] = 0x2212,
            ['\\'] = 0x1100,
            [']'] = 0x2221,
            ['^'] = 0x5000,
            ['_'] = 0x0030,
            ['`'] = 0x0100,
            ['a'] = 0xA070,
            ['b'] = 0xA0E0,
            ['c'] = 0x8060,
            ['d'] = 0x281C,
            ['e'] = 0xC060,
            ['f'] = 0xAA02,
            ['g'] = 0xA2A1,
            ['h'] = 0xA0C0,
            ['i'] = 0x2000,
            ['j'] = 0x2260,
            ['k'] = 0x3600,
            ['l'] = 0x00C0,
            ['m'] = 0xA848,
            ['n'] = 0xA040,
            ['o'] = 0xA060,
            ['p'] = 0x82C1,
            ['q'] = 0xA281,
            ['r'] = 0x8040,
            ['s'] = 0xA0A1,
            ['t'] = 0x80E0,
            ['u'] = 0x2060,
            ['v'] = 0x4040,
            ['w'] = 0x5048,
            ['x'] = 0x5500,
            ['y'] = 0x0A1C,
            ['z'] = 0xC020,
            ['{'] = 0xA212,
            ['|'] = 0x2200,
            ['}'] = 0x2A21,
            ['~'] = 0xCC00
        };

        public static bool[] GetBitsSixteen(this char c)
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

        private static bool TryGetMask(char original, char normalized, out ushort mask)
        {
            if (SixteenMasks.TryGetValue(original, out mask))
            {
                return true;
            }

            return SixteenMasks.TryGetValue(normalized, out mask);
        }
    }
}