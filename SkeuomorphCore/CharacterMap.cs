using System;
using System.Collections.Generic;

namespace SkeuomorphCore
{
    public sealed class CharacterMap
    {
        private readonly Dictionary<char, ulong> _masks = new();

        public CharacterMap(int width, int height)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Map width must be greater than zero.");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Map height must be greater than zero.");
            }

            if (width * height > sizeof(ulong) * 8)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Map segment count exceeds the ulong bitmask capacity for this layout.");
            }

            Width = width;
            Height = height;
        }

        public int Width { get; }

        public int Height { get; }

        public int SegmentCount => Width * Height;

        public IReadOnlyDictionary<char, ulong> Masks => _masks;

        public void Set(char c, ulong mask)
        {
            ulong clamped = SegmentCount == 64 ? mask : (mask & ((1UL << SegmentCount) - 1UL));
            _masks[c] = clamped;
        }

        public void Set(char c, int mask)
        {
            Set(c, (ulong)mask);
        }

        public void Set(char c, bool[] bits)
        {
            if (bits is null)
            {
                throw new ArgumentNullException(nameof(bits));
            }

            if (bits.Length != SegmentCount)
            {
                throw new ArgumentException($"Bit array length must be {SegmentCount} for a {Width}x{Height} map.", nameof(bits));
            }

            _masks[c] = MaskFromBits(bits);
        }

        public void Set(char c, params string[] rows)
        {
            if (rows is null)
            {
                throw new ArgumentNullException(nameof(rows));
            }

            if (rows.Length != Height)
            {
                throw new ArgumentException($"Expected {Height} rows, received {rows.Length}.", nameof(rows));
            }

            var bits = new bool[SegmentCount];
            for (var row = 0; row < Height; row++)
            {
                var line = rows[row];
                if (line.Length != Width)
                {
                    throw new ArgumentException($"Row {row} must contain exactly {Width} columns.", nameof(rows));
                }

                for (var column = 0; column < Width; column++)
                {
                    var value = line[column];
                    bits[(row * Width) + column] = value == '1' || value == 'X' || value == 'x';
                }
            }

            Set(c, bits);
        }

        public bool TryGetMask(char c, out ulong mask)
        {
            return _masks.TryGetValue(c, out mask);
        }

        public ulong GetMask(char c)
        {
            return _masks.TryGetValue(c, out var mask) ? mask : 0UL;
        }

        public bool[] GetBits(char c)
        {
            return TryGetMask(c, out var mask) ? BitsFromMask(mask, SegmentCount) : new bool[SegmentCount];
        }

        public bool Remove(char c)
        {
            return _masks.Remove(c);
        }

        public static ulong MaskFromBits(ReadOnlySpan<bool> bits)
        {
            ulong mask = 0;
            for (var index = 0; index < bits.Length; index++)
            {
                if (bits[index])
                {
                    mask |= 1UL << index;
                }
            }

            return mask;
        }

        public static bool[] BitsFromMask(ulong mask, int segmentCount)
        {
            if (segmentCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(segmentCount), "Segment count must be greater than zero.");
            }

            if (segmentCount > sizeof(ulong) * 8)
            {
                throw new ArgumentOutOfRangeException(nameof(segmentCount), "Segment count exceeds the size of a ulong bitmask.");
            }

            var bits = new bool[segmentCount];
            for (var index = 0; index < segmentCount; index++)
            {
                bits[index] = ((mask >> index) & 1UL) == 1UL;
            }

            return bits;
        }
    }
}
