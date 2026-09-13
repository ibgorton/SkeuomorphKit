using System.Collections.Generic;

namespace SkeuomorphCore;

public sealed class FourteenSegmentDisplayProfile : DisplayProfileBase
{
    private static readonly IGlyphMap BuiltInMap = DisplayCharacterProfiles.GetMap("FourteenSegment");
    private static readonly HashSet<char> SupportedCharacters = BuildFourteenSegmentSet();

    public FourteenSegmentDisplayProfile() : base("FourteenSegment", 14, 4, 4, SupportedCharacters, BuiltInMap)
    {
    }

    public override bool[] GetBits(char c)
    {
        return BuiltInMap switch
        {
            ISegmentedGlyphMap segmented => segmented.GetBits(c),
            _ => new bool[14]
        };
    }

    private static HashSet<char> BuildFourteenSegmentSet()
    {
        return [.. BuiltInMap.SupportedCharacters];
    }
}
