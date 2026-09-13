using System;

namespace SkeuomorphCore;

public static class SegmentMapExtensions
{
    public static bool[] GetBits(this char c, IDisplayProfile profile)
    {
        if (profile is null)
        {
            throw new ArgumentNullException(nameof(profile));
        }

        return profile.GetBits(c);
    }

    public static bool[] GetBits(this char c, string mapName)
    {
        if (string.IsNullOrWhiteSpace(mapName))
        {
            throw new ArgumentException("A map name is required.", nameof(mapName));
        }

        return DisplayCharacterProfiles.TryGetMap(mapName, out var map) && map is ISegmentedGlyphMap segmented
            ? segmented.GetBits(c)
            : Array.Empty<bool>();
    }

    public static bool[] GetBits(this char c, IGlyphMap map)
    {
        if (map is null)
        {
            throw new ArgumentNullException(nameof(map));
        }

        return map is ISegmentedGlyphMap segmented
            ? segmented.GetBits(c)
            : Array.Empty<bool>();
    }
}
