using System.Collections.Generic;

namespace SkeuomorphCore;

public sealed class SixteenSegmentDisplayProfile : DisplayProfileBase
{
    private static readonly IGlyphMap BuiltInMap = DisplayCharacterProfiles.GetMap("SixteenSegment");
    private static readonly HashSet<char> SupportedCharacters = BuildSixteenSegmentSet();

    public SixteenSegmentDisplayProfile() : base("SixteenSegment", 16, 4, 4, SupportedCharacters, BuiltInMap)
    {
    }

    public override bool[] GetBits(char c)
    {
        return BuiltInMap switch
        {
            ISegmentedGlyphMap segmented => segmented.GetBits(c),
            _ => new bool[16]
        };
    }

    private static HashSet<char> BuildSixteenSegmentSet()
    {
        return [.. BuiltInMap.SupportedCharacters];
    }
}
