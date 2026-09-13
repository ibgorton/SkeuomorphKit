using System.Collections.Generic;

namespace SkeuomorphCore;

internal static class BitmapGlyphSources
{
    public static IReadOnlyDictionary<char, BitmapGlyph> IbmBios8x8 => IbmBios8x8GlyphSource.Glyphs;
    public static IReadOnlyDictionary<char, BitmapGlyph> IbmCga8x8 => IbmCga8x8GlyphSource.Glyphs;
    public static IReadOnlyDictionary<char, BitmapGlyph> IbmCgaLight8x8 => IbmCgaLight8x8GlyphSource.Glyphs;
}
