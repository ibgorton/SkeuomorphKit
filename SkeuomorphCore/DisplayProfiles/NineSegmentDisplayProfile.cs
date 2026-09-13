using System.Collections.Generic;

namespace SkeuomorphCore;

public sealed class NineSegmentDisplayProfile : DisplayProfileBase
{
    private static readonly IGlyphMap BuiltInMap = DisplayCharacterProfiles.GetMap("NineSegmentSlash");
    private static readonly HashSet<char> SupportedCharacters = BuildNineSegmentSet();

    public NineSegmentDisplayProfile(string name = "NineSegmentSlash") : base(name, 9, 3, 3, SupportedCharacters, BuiltInMap)
    {
    }

    public override bool[] GetBits(char c)
    {
        var map = DisplayCharacterProfiles.TryGetMap(Name, out var found)
            ? found
            : BuiltInMap;

        return map switch
        {
            ISegmentedGlyphMap segmented => segmented.GetBits(c),
            _ => new bool[9]
        };
    }

    private static HashSet<char> BuildNineSegmentSet()
    {
        return [.. BuiltInMap.SupportedCharacters];
    }
}
