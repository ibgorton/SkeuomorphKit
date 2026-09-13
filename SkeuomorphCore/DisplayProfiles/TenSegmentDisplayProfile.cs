using System.Collections.Generic;

namespace SkeuomorphCore;

public sealed class TenSegmentDisplayProfile : DisplayProfileBase
{
    private static readonly HashSet<char> SupportedCharacters = BuildTenSegmentSet();

    public TenSegmentDisplayProfile() : base("TenSegment", 10, 3, 3, SupportedCharacters)
    {
    }

    public override bool[] GetBits(char c)
    {
        return new TenMap().GetBits(c);
    }

    private static HashSet<char> BuildTenSegmentSet()
    {
        return [.. TenMap.SupportedCharacters];
    }
}
