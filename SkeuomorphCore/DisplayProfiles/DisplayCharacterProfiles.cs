using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        if (!TryGet(profileName, out var profile))
        {
            return false;
        }

        var mapType = GetMapType(profileName);
        if (mapType is null || mapType.IsAbstract || !typeof(SegmentMapBase).IsAssignableFrom(mapType))
        {
            return profile.IsSupported(c);
        }

        var mapInstance = Activator.CreateInstance(mapType);
        var masksProperty = mapType.GetProperty("Masks", BindingFlags.Public | BindingFlags.Instance);
        if (masksProperty is null || masksProperty.GetValue(mapInstance) is not System.Collections.IDictionary dictionary)
        {
            return profile.IsSupported(c);
        }

        if (!dictionary.Contains(c))
        {
            return false;
        }

        var value = dictionary[c];
        return value is not null;
    }

    public static void SetCharacterEnabled(string profileName, char c, bool enabled)
    {
        if (!TryGet(profileName, out _))
        {
            throw new KeyNotFoundException($"Display profile '{profileName}' was not found.");
        }

        var mapType = GetMapType(profileName);
        if (mapType is null || mapType.IsAbstract || !typeof(SegmentMapBase).IsAssignableFrom(mapType))
        {
            return;
        }

        var mapInstance = Activator.CreateInstance(mapType);
        var masksProperty = mapType.GetProperty("Masks", BindingFlags.Public | BindingFlags.Instance);
        if (masksProperty is null || masksProperty.GetValue(mapInstance) is not System.Collections.IDictionary dictionary)
        {
            return;
        }

        var defaultMasksField = mapType.GetField("DefaultMasks", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        if (enabled)
        {
            if (defaultMasksField is not null && defaultMasksField.GetValue(null) is System.Collections.IDictionary defaults && defaults.Contains(c))
            {
                dictionary[c] = defaults[c];
            }
            else
            {
                dictionary.Remove(c);
            }

            return;
        }

        dictionary[c] = null;
    }

    public static bool IsSupported(string profileName, char c)
    {
        return TryGet(profileName, out var profile) && IsCharacterEnabled(profileName, c);
    }

    public static bool IsSupported(IDisplayProfile profile, char c)
    {
        return profile is not null && IsCharacterEnabled(profile.Name, c);
    }

    private static Type GetMapType(string profileName)
    {
        var typeName = profileName switch
        {
            "SevenSegment" => "SevenMap",
            "NineSegment" => "NineMap",
            "TenSegment" => "TenMap",
            "FourteenSegment" => "FourteenMap",
            "Rectangle5x7" => "RectangleMap",
            "DotMatrix8x8" => "DotMatrix8x8",
            "SixteenSegment" => "SixteenMap",
            _ => null
        };

        if (typeName is null)
        {
            return null;
        }

        return typeof(DisplayCharacterProfiles).Assembly.GetType($"SkeuomorphCore.{typeName}");
    }

}
