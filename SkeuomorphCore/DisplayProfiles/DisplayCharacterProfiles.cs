using System;
using System.Collections.Generic;
using System.Linq;

namespace SkeuomorphCore;

public static class DisplayCharacterProfiles
{
    private static readonly Dictionary<string, IDisplayProfile> Profiles = new(StringComparer.OrdinalIgnoreCase)
    {
        ["SevenSegment"] = new SevenSegmentDisplayProfile(),
        ["NineSegment"] = new NineSegmentDisplayProfile(),
        ["TenSegment"] = new TenSegmentDisplayProfile(),
        ["FourteenSegment"] = new FourteenSegmentDisplayProfile(),
        ["Rectangle5x7"] = new Rectangle5x7DisplayProfile(),
        ["DotMatrix8x8"] = new DotMatrix8x8DisplayProfile(),
        ["SixteenSegment"] = new SixteenSegmentDisplayProfile()
    };

    private static readonly Dictionary<string, IGlyphMap> Maps = new(StringComparer.OrdinalIgnoreCase)
    {
        ["SevenSegment"] = new SevenMap(),
        ["NineSegment"] = new NineMap(),
        ["TenSegment"] = new TenMap(),
        ["FourteenSegment"] = new FourteenMap(),
        ["Rectangle5x7"] = new RectangleMap(),
        ["DotMatrix8x8"] = new DotMatrix8x8Map(),
        ["SixteenSegment"] = new SixteenMap()
    };

    public static IReadOnlyCollection<string> AllNames => Profiles.Keys.ToArray();

    public static IReadOnlyCollection<IDisplayProfile> AllProfiles => Profiles.Values.ToArray();

    public static void Register(IDisplayProfile profile)
    {
        if (profile is null)
        {
            throw new ArgumentNullException(nameof(profile));
        }

        Profiles[profile.Name] = profile;
    }

    public static IDisplayProfile Get(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A profile name is required.", nameof(name));
        }

        if (!Profiles.TryGetValue(name, out var profile))
        {
            throw new KeyNotFoundException($"Display profile '{name}' was not found.");
        }

        return profile;
    }

    public static bool TryGet(string name, out IDisplayProfile profile)
    {
        return Profiles.TryGetValue(name ?? string.Empty, out profile!);
    }

    public static bool IsCharacterEnabled(string profileName, char c)
    {
        if (Maps.TryGetValue(profileName ?? string.Empty, out var map))
        {
            return map.IsSupported(c);
        }

        return TryGet(profileName, out var profile) && profile.IsSupported(c);
    }

    public static void SetCharacterEnabled(string profileName, char c, bool enabled)
    {
        if (!TryGet(profileName, out _))
        {
            throw new KeyNotFoundException($"Display profile '{profileName}' was not found.");
        }

        if (!Maps.TryGetValue(profileName ?? string.Empty, out var map))
        {
            return;
        }

        map.SetCharacterEnabled(c, enabled);
    }

    public static bool IsSupported(string profileName, char c)
    {
        return TryGet(profileName, out var profile) && IsCharacterEnabled(profileName, c);
    }

    public static bool IsSupported(IDisplayProfile profile, char c)
    {
        return profile is not null && IsCharacterEnabled(profile.Name, c);
    }
}
