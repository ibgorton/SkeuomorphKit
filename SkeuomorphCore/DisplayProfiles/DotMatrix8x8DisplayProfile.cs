using System.Collections.Generic;

namespace SkeuomorphCore;

public sealed class DotMatrix8x8DisplayProfile : DisplayProfileBase
{
    private static readonly IBitmapGlyphMap BuiltInMap = (IBitmapGlyphMap)DisplayCharacterProfiles.GetMap("DotMatrix8x8");
    private static readonly HashSet<char> SupportedCharacters = BuildDotMatrixSet();

    public DotMatrix8x8DisplayProfile() : base("DotMatrix8x8", 64, 8, 8, SupportedCharacters, BuiltInMap)
    {
    }

    public override bool[] GetBits(char c)
    {
        if (!BuiltInMap.TryGetGlyph(c, out var glyph))
        {
            return new bool[64];
        }

        var bits = new bool[BuiltInMap.Width * BuiltInMap.Height];
        for (var row = 0; row < BuiltInMap.Height; row++)
        {
            for (var col = 0; col < BuiltInMap.Width; col++)
            {
                bits[(row * BuiltInMap.Width) + col] = glyph.HasPixel(col, row);
            }
        }

        return bits;
    }

    private static HashSet<char> BuildDotMatrixSet()
    {
        return [.. BuiltInMap.SupportedCharacters];
    }
}
