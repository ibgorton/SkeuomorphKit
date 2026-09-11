using System;
using System.Collections.Generic;
using System.Linq;

namespace SkeuomorphCore
{
    // Canonical bit ordering follows the dmadison/led-segment-ascii reference set:
    // 7-segment: DP-G-F-E-D-C-B-A
    // See https://github.com/dmadison/led-segment-ascii
    // Licensed under the MIT license (Copyright © 2017 David Madison).
    public static class SevenMap
    {
        private const int SegmentCount = 7;

        // Compact byte-based masks keep the same host-order semantics while eliminating the giant bool[][] tables.
        private static readonly Dictionary<char, byte> SevenMasks = new()
        {
            [' '] = 0x00,
            ['-'] = 0x40,
            ['.'] = 0x02,
            [':'] = 0x02,
            ['0'] = 0x3F,
            ['1'] = 0x06,
            ['2'] = 0x5B,
            ['3'] = 0x4F,
            ['4'] = 0x66,
            ['5'] = 0x6D,
            ['6'] = 0x7D,
            ['7'] = 0x07,
            ['8'] = 0x7F,
            ['9'] = 0x6F,
            ['='] = 0x48,
            ['+'] = 0x48,
            ['/'] = 0x02,
            ['A'] = 0x77,
            ['B'] = 0x7C,
            ['C'] = 0x39,
            ['D'] = 0x5E,
            ['E'] = 0x79,
            ['F'] = 0x71,
            ['G'] = 0x3D,
            ['H'] = 0x76,
            ['I'] = 0x06,
            ['J'] = 0x1E,
            ['K'] = 0x76,
            ['L'] = 0x39,
            ['M'] = 0x76,
            ['N'] = 0x76,
            ['O'] = 0x3F,
            ['P'] = 0x77,
            ['Q'] = 0x7E,
            ['R'] = 0x77,
            ['S'] = 0x6D,
            ['T'] = 0x40,
            ['U'] = 0x3E,
            ['V'] = 0x3E,
            ['W'] = 0x76,
            ['X'] = 0x76,
            ['Y'] = 0x66,
            ['Z'] = 0x5B,
            ['_'] = 0x08,
            ['r'] = 0x50,
            ['o'] = 0x5C
        };

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
}