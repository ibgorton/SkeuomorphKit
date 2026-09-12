using System;
using System.Collections.Generic;

namespace SkeuomorphCore;

/// <summary>
/// Represents a mapping of characters to segment masks for a display layout.
/// </summary>
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

    /// <summary>
    /// Associates a character with a segment mask for the current display layout.
    /// </summary>
    /// <param name="c">The character to assign.</param>
    /// <param name="mask">The bitmask to store for the character.</param>
    /// <remarks>
    /// Bits outside the current layout's segment count are discarded to keep the mask within the valid range.
    /// </remarks>
    public void Set(char c, ulong mask)
    {
        ulong clamped = SegmentCount == 64 ? mask : (mask & ((1UL << SegmentCount) - 1UL));
        _masks[c] = clamped;
    }

    /// <summary>
    /// Associates a character with a segment mask using an integer value.
    /// </summary>
    /// <param name="c">The character to assign.</param>
    /// <param name="mask">The integer bitmask to store for the character.</param>
    /// <remarks>
    /// The integer value is converted to an unsigned long before being normalized to the current layout.
    /// </remarks>
    public void Set(char c, int mask)
    {
        Set(c, (ulong)mask);
    }

    /// <summary>
    /// Associates a character with a segment mask represented as a boolean array.
    /// </summary>
    /// <param name="c">The character to assign.</param>
    /// <param name="bits">A boolean array whose length matches the current segment count.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="bits"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the array length does not match the current layout size.</exception>
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

    /// <summary>
    /// Associates a character with a segment mask described by row strings.
    /// </summary>
    /// <param name="c">The character to assign.</param>
    /// <param name="rows">Rows of the layout using '1', 'X', or 'x' for active segments.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="rows"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the row count or row lengths do not match the current layout dimensions.</exception>
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

    /// <summary>
    /// Tries to retrieve the mask assigned to a character.
    /// </summary>
    /// <param name="c">The character to query.</param>
    /// <param name="mask">When this method returns true, contains the bitmask for the character.</param>
    /// <returns><see langword="true"/> when the character exists in the map; otherwise, <see langword="false"/>.</returns>
    public bool TryGetMask(char c, out ulong mask)
    {
        return _masks.TryGetValue(c, out mask);
    }

    /// <summary>
    /// Gets the segment mask for a character, or zero when the character is not defined.
    /// </summary>
    /// <param name="c">The character to query.</param>
    /// <returns>The bitmask assigned to the character, or <c>0UL</c> if it is not present.</returns>
    public ulong GetMask(char c)
    {
        return _masks.TryGetValue(c, out var mask) ? mask : 0UL;
    }

    /// <summary>
    /// Gets the segment state for a character as a boolean array.
    /// </summary>
    /// <param name="c">The character to query.</param>
    /// <returns>A boolean array representing the character's active segments, or all-false values when the character is not present.</returns>
    public bool[] GetBits(char c)
    {
        return TryGetMask(c, out var mask) ? BitsFromMask(mask, SegmentCount) : new bool[SegmentCount];
    }

    /// <summary>
    /// Removes the mapping for a character if it exists.
    /// </summary>
    /// <param name="c">The character to remove.</param>
    /// <returns><see langword="true"/> when the character was present and removed; otherwise, <see langword="false"/>.</returns>
    public bool Remove(char c)
    {
        return _masks.Remove(c);
    }

    /// <summary>
    /// Converts an ordered sequence of segment flags into a bitmask.
    /// </summary>
    /// <param name="bits">The segment state sequence, where <see langword="true"/> indicates an active segment.</param>
    /// <returns>A bitmask representing the active segments.</returns>
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

    /// <summary>
    /// Converts a bitmask into a boolean array of segment states.
    /// </summary>
    /// <param name="mask">The bitmask to decode.</param>
    /// <param name="segmentCount">The number of segments to decode.</param>
    /// <returns>A boolean array where each item corresponds to a segment bit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the segment count is less than or equal to zero or exceeds the capacity of a <see cref="ulong"/>.</exception>
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
