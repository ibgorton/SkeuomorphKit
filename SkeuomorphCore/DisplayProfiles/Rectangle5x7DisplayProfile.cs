using System.Collections.Generic;

namespace SkeuomorphCore;

public sealed class Rectangle5x7DisplayProfile : DisplayProfileBase
{
    private static readonly IGlyphMap BuiltInMap = DisplayCharacterProfiles.GetMap("Rectangle5x7");
    private static readonly HashSet<char> SupportedCharacters = BuildRectangle5x7Set();

    public Rectangle5x7DisplayProfile() : base("Rectangle5x7", GlyphLibrary.Width * GlyphLibrary.Height, GlyphLibrary.Width, GlyphLibrary.Height, SupportedCharacters, BuiltInMap)
    {
    }

    public override bool[] GetBits(char c)
    {
        return BuiltInMap switch
        {
            ISegmentedGlyphMap segmented => segmented.GetBits(c),
            _ => new bool[GlyphLibrary.Width * GlyphLibrary.Height]
        };
    }

    private static HashSet<char> BuildRectangle5x7Set()
    {
        return [.. BuiltInMap.SupportedCharacters];
    }
}
