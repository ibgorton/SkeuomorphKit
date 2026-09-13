using System;

namespace SkeuomorphCore;

public static class DotMatrix8x8
{
    public static bool[] GetBitsDotMatrix8x8(this char c)
    {
        return DotMatrix8x8Map.GetBits(c, DotMatrix8x8GlyphStyles.Default);
    }
}
