using System.Collections.Generic;

namespace SkeuomorphCore;

public sealed class TenSegmentDisplayProfile : DisplayProfileBase
{
    private static readonly IGlyphMap BuiltInMap = DisplayCharacterProfiles.GetMap("TenSegment");
    private static readonly HashSet<char> SupportedCharacters = BuildTenSegmentSet();

    public TenSegmentDisplayProfile() : base("TenSegment", 10, 3, 3, SupportedCharacters, BuiltInMap)
    {
    }

    public override bool[] GetBits(char c)
    {
        return BuiltInMap switch
        {
            ISegmentedGlyphMap segmented => segmented.GetBits(c),
            _ => new bool[10]
        };
    }

    private static HashSet<char> BuildTenSegmentSet()
    {
        return [.. BuiltInMap.SupportedCharacters];
    }
}
