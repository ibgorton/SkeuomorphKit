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

        var normalized = NormalizeCharacter(c);
        return profile.IsSupported(normalized) && !GetDisabledCharacters(profileName).Contains(normalized);
    }

    public static void SetCharacterEnabled(string profileName, char c, bool enabled)
    {
        if (!TryGet(profileName, out _))
        {
            throw new KeyNotFoundException($"Display profile '{profileName}' was not found.");
        }

        var normalized = NormalizeCharacter(c);
        var disabled = GetDisabledCharacters(profileName);
        if (enabled)
        {
            disabled.Remove(normalized);
        }
        else
        {
            disabled.Add(normalized);
        }

        var mapType = GetMapType(profileName);
        if (mapType is null)
        {
            return;
        }

        var field = mapType.GetField("DisabledCharacters", BindingFlags.Public | BindingFlags.Static);
        if (field is not null && field.FieldType == typeof(HashSet<char>))
        {
            field.SetValue(null, disabled);
        }
    }

    public static bool IsSupported(string profileName, char c)
    {
        return TryGet(profileName, out var profile) && IsCharacterEnabled(profileName, c);
    }

    public static bool IsSupported(IDisplayProfile profile, char c)
    {
        return profile is not null && IsCharacterEnabled(profile.Name, c);
    }

    private static HashSet<char> GetDisabledCharacters(string profileName)
    {
        var mapType = GetMapType(profileName);
        if (mapType is null)
        {
            return new HashSet<char>();
        }

        var field = mapType.GetField("DisabledCharacters", BindingFlags.Public | BindingFlags.Static);
        if (field is null || field.GetValue(null) is not IEnumerable<char> values)
        {
            return new HashSet<char>();
        }

        return new HashSet<char>(values.Select(NormalizeCharacter));
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

    private static char NormalizeCharacter(char c)
    {
        return char.ToUpperInvariant(c);
    }
}
