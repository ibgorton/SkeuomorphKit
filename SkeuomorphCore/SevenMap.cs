using System;
using System.Collections.Generic;
using System.Linq;

namespace SkeuomorphCore
{
    public static class SevenMap
    {
        private const int SegmentCount = 7;

        private static readonly Dictionary<char, bool[]> SevenBits = new()
        {
            { '-', new bool[SegmentCount] { true, false, false, false, false, false, false } },
            { '0', new bool[SegmentCount] { false, true, true, true, true, true, true } },
            { '1', new bool[SegmentCount] { false, false, false, false, true, true, false } },
            { '2', new bool[SegmentCount] { true, false, true, true, false, true, true } },
            { '3', new bool[SegmentCount] { true, false, false, true, true, true, true } },
            { '4', new bool[SegmentCount] { true, true, false, false, true, true, false } },
            { '5', new bool[SegmentCount] { true, true, false, true, true, false, true } },
            { '6', new bool[SegmentCount] { true, true, true, true, true, false, true } },
            { '7', new bool[SegmentCount] { false, false, false, false, true, true, true } },
            { '8', new bool[SegmentCount] { true, true, true, true, true, true, true } },
            { '9', new bool[SegmentCount] { true, true, false, true, true, true, true } },
            { '=', new bool[SegmentCount] { true, false, false, true, false, false, false } },
            { 'A', new bool[SegmentCount] { true, true, true, false, true, true, true } },
            { 'B', new bool[SegmentCount] { true, true, true, true, true, false, false } },
            { 'C', new bool[SegmentCount] { false, true, true, true, false, false, true } },
            { 'D', new bool[SegmentCount] { true, false, true, true, true, true, false } },
            { 'E', new bool[SegmentCount] { true, true, true, true, false, false, true } },
            { 'F', new bool[SegmentCount] { true, true, true, false, false, false, true } },
            { 'G', new bool[SegmentCount] { false, true, true, true, true, false, true } },
            { 'H', new bool[SegmentCount] { true, true, true, false, true, true, false } },
            { '_', new bool[SegmentCount] { false, false, false, true, false, false, false } },
            { 'r', new bool[SegmentCount] { true, false, true, false, false, false, false } },
            { 'o', new bool[SegmentCount] { true, false, true, true, true, false, false } }
        };

        public static bool[] GetBitsSeven(this char c)
        {
            var result = new bool[SegmentCount];
            if (!SevenBits.TryGetValue(c, out bool[] bits))
                return result;

            for (int i = 0; i < SegmentCount; i++)
            {
                result[i] = bits[i];
            }

            return result;
        }

        public static void GetBitSeven(this bool[] t, char c)
        {
            if (t is null)
                throw new ArgumentNullException(nameof(t));

            Array.Clear(t, 0, t.Length);

            if (!SevenBits.TryGetValue(c, out bool[] bits))
                return;

            for (int i = 0; i < SegmentCount; i++)
            {
                t[i] = bits[i];
            }
        }
    }
}