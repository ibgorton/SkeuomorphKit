using System.Collections.Generic;

namespace SkeuomorphCore;

public sealed class NineSegmentDisplayProfile : DisplayProfileBase
{
    private static readonly HashSet<char> SupportedCharacters = BuildNineSegmentSet();

    public NineSegmentDisplayProfile() : base("NineSegment", 9, 3, 3, SupportedCharacters)
    {
    }

    public override bool[] GetBits(char c)
    {
        return c.GetBitsNine();
    }

    private static HashSet<char> BuildNineSegmentSet()
    {
        return [.. NineMap.SupportedCharacters];
    }
}
