using System;

namespace SkeuomorphCore;

public static class DotMatrix8x8
{
    private const int Width = 8;
    private const int Height = 8;
    private const int SegmentCount = Width * Height;

    public static bool[] GetBitsDotMatrix8x8(this char c)
    {
        if (!GlyphLibrary.TryGetPattern(c, out var rows))
        {
            return new bool[SegmentCount];
        }

        var result = new bool[SegmentCount];
        for (var row = 0; row < Height; row++)
        {
            for (var col = 0; col < Width; col++)
            {
                if (row == 0 || col == 0 || col > 5)
                {
                    continue;
                }

                var glyphRow = row - 1;
                var glyphCol = col - 1;
                if (glyphRow < 0 || glyphRow >= 7 || glyphCol < 0 || glyphCol >= 5)
                {
                    continue;
                }

                result[(row * Width) + col] = rows[glyphRow][glyphCol] == '1';
            }
        }

        return result;
    }
}
