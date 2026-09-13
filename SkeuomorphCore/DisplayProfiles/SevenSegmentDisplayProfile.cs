using System.Collections.Generic;

namespace SkeuomorphCore;

public sealed class SevenSegmentDisplayProfile : DisplayProfileBase
{
    private static readonly IGlyphMap BuiltInMap = DisplayCharacterProfiles.GetMap("SevenSegment");
    private static readonly HashSet<char> SupportedCharacters = BuildSevenSegmentSet();

    public SevenSegmentDisplayProfile() : base("SevenSegment", 7, 7, 1, SupportedCharacters, BuiltInMap)
    {
    }

    public override bool[] GetBits(char c)
    {
        return BuiltInMap switch
        {
            ISegmentedGlyphMap segmented => segmented.GetBits(c),
            _ => new bool[7]
        };
    }

    private static HashSet<char> BuildSevenSegmentSet()
    {
        return [.. BuiltInMap.SupportedCharacters];
    }
}
