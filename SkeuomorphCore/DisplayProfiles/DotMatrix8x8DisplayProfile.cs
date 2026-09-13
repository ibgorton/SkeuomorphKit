using System.Collections.Generic;

namespace SkeuomorphCore;

public sealed class DotMatrix8x8DisplayProfile : DisplayProfileBase
{
    private static readonly HashSet<char> SupportedCharacters = BuildDotMatrixSet();

    public DotMatrix8x8DisplayProfile() : base("DotMatrix8x8", 64, 8, 8, SupportedCharacters)
    {
    }

    public override bool[] GetBits(char c)
    {
        return DotMatrix8x8Map.GetBits(c, DotMatrix8x8GlyphStyles.Default);
    }

    private static HashSet<char> BuildDotMatrixSet()
    {
        return [.. DotMatrix8x8Map.SupportedCharacters];
    }
}
