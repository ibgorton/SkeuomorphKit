using System.Collections.Generic;

namespace SkeuomorphCore;

public sealed class SevenSegmentDisplayProfile : DisplayProfileBase
{
    private static readonly HashSet<char> SupportedCharacters = BuildSevenSegmentSet();

    public SevenSegmentDisplayProfile() : base("SevenSegment", 7, 7, 1, SupportedCharacters)
    {
    }

    public override bool[] GetBits(char c)
    {
        return c.GetBitsSeven();
    }

    private static HashSet<char> BuildSevenSegmentSet()
    {
        return [.. SevenMap.SupportedCharacters];
    }
}
