using System;
using System.Collections.Generic;
using System.Linq;

namespace SkeuomorphCore;

public static class DisplayCharacterProfiles
{
    private sealed class GlyphMapDisplayProfile : IDisplayProfile
    {
        private readonly ISegmentedGlyphMap? _segmentMap;
        private readonly IBitmapGlyphMap? _bitmapMap;

        public GlyphMapDisplayProfile(IGlyphMap map)
        {
            if (map is null)
            {
                throw new ArgumentNullException(nameof(map));
            }

            Name = map.Name;
            if (map is ISegmentedGlyphMap segmented)
            {
                _segmentMap = segmented;
                SegmentCount = segmented.MapSegmentCount;
                Width = 1;
                Height = Math.Max(1, segmented.MapSegmentCount);
                return;
            }

            if (map is IBitmapGlyphMap bitmap)
            {
                _bitmapMap = bitmap;
                SegmentCount = bitmap.Width * bitmap.Height;
                Width = bitmap.Width;
                Height = bitmap.Height;
                return;
            }

            throw new NotSupportedException($"The glyph map type '{map.GetType().FullName}' is not supported as a display profile.");
        }

        public string Name { get; }

        public int SegmentCount { get; }

        public int Width { get; }

        public int Height { get; }

        public bool IsSupported(char c)
        {
            return _segmentMap?.IsSupported(c) == true || _bitmapMap?.IsSupported(c) == true;
        }

        public bool[] GetBits(char c)
        {
            if (_segmentMap is not null)
            {
                return _segmentMap.GetBits(c);
            }

            if (_bitmapMap is not null && _bitmapMap.TryGetGlyph(c, out var glyph))
            {
                var bits = new bool[glyph.Width * glyph.Height];
                for (var row = 0; row < glyph.Height; row++)
                {
                    for (var col = 0; col < glyph.Width; col++)
                    {
                        bits[(row * glyph.Width) + col] = glyph.HasPixel(col, row);
                    }
                }

                return bits;
            }

            return new bool[SegmentCount];
        }
    }

    private static readonly Dictionary<string, IGlyphMap> Maps = BuildBuiltInMaps();
    private static readonly Dictionary<string, IDisplayProfile> Profiles = BuildProfiles();

    private static Dictionary<string, IDisplayProfile> BuildProfiles()
    {
        return new Dictionary<string, IDisplayProfile>(StringComparer.OrdinalIgnoreCase)
        {
            ["SevenSegment"] = new SevenSegmentDisplayProfile(),
            ["NineSegmentSlash"] = new NineSegmentDisplayProfile("NineSegmentSlash"),
            ["NineSegmentBackslash"] = new NineSegmentDisplayProfile("NineSegmentBackslash"),
            ["NineSegmentSlashAlt"] = new NineSegmentDisplayProfile("NineSegmentSlashAlt"),
            ["NineSegmentBackslashAlt"] = new NineSegmentDisplayProfile("NineSegmentBackslashAlt"),
            ["TenSegment"] = new TenSegmentDisplayProfile(),
            ["FourteenSegment"] = new FourteenSegmentDisplayProfile(),
            ["Rectangle5x7"] = new Rectangle5x7DisplayProfile(),
            ["DotMatrix8x8"] = new DotMatrix8x8DisplayProfile(),
            ["SixteenSegment"] = new SixteenSegmentDisplayProfile()
        };
    }

    private static Dictionary<string, IGlyphMap> BuildBuiltInMaps()
    {
        var maps = new Dictionary<string, IGlyphMap>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in GlyphMapCatalog.BuiltInJson)
        {
            maps[pair.Key] = CreateMapFromJson(pair.Key, pair.Value);
        }

        return maps;
    }

    private static IGlyphMap CreateMapFromJson(string name, string json)
    {
        var definition = GlyphMapDefinition.FromJson(json, name);
        return definition.Kind switch
        {
            GlyphMapKind.Bitmap => new JsonBitmapGlyphMap(definition.Name, definition.ToBitmaps(), definition.SegmentCount),
            _ => new JsonGlyphMap(definition.Name, definition.ToMasks(), definition.SegmentCount)
        };
    }

    public static IReadOnlyDictionary<string, ISegmentedGlyphMap> SegmentMaps => Maps
        .Where(static pair => pair.Value is ISegmentedGlyphMap)
        .ToDictionary(static pair => pair.Key, static pair => (ISegmentedGlyphMap)pair.Value, StringComparer.OrdinalIgnoreCase);

    public static IReadOnlyDictionary<string, IBitmapGlyphMap> BitmapMaps => Maps
        .Where(static pair => pair.Value is IBitmapGlyphMap)
        .ToDictionary(static pair => pair.Key, static pair => (IBitmapGlyphMap)pair.Value, StringComparer.OrdinalIgnoreCase);

    static DisplayCharacterProfiles()
    {
        foreach (var pair in GlyphMapCatalog.BuiltInJson)
        {
            if (!Maps.ContainsKey(pair.Key))
            {
                RegisterMapJson(pair.Key, pair.Value);
            }
        }
    }

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

    public static void RegisterMap(string name, IGlyphMap map)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A map name is required.", nameof(name));
        }

        if (map is null)
        {
            throw new ArgumentNullException(nameof(map));
        }

        Maps[name] = map;
        if (!Profiles.ContainsKey(name))
        {
            Profiles[name] = new GlyphMapDisplayProfile(map);
        }
    }

    public static void RegisterMapJson(string name, string json)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A map name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("A glyph map JSON payload is required.", nameof(json));
        }

        var definition = GlyphMapDefinition.FromJson(json, name);
        IGlyphMap map = definition.Kind switch
        {
            GlyphMapKind.Bitmap => new JsonBitmapGlyphMap(definition.Name, definition.ToBitmaps(), definition.SegmentCount),
            _ => new JsonGlyphMap(definition.Name, definition.ToMasks(), definition.SegmentCount)
        };

        RegisterMap(definition.Name, map);
    }

    public static IReadOnlyDictionary<string, string> GetBuiltInMapJson()
    {
        return GlyphMapCatalog.BuiltInJson;
    }

    private static string NormalizeMapName(string? name)
    {
        return string.IsNullOrWhiteSpace(name) ? string.Empty : name.Trim();
    }

    public static IDisplayProfile Get(string name)
    {
        var normalized = NormalizeMapName(name);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("A profile name is required.", nameof(name));
        }

        if (!Profiles.TryGetValue(normalized, out var profile))
        {
            throw new KeyNotFoundException($"Display profile '{normalized}' was not found.");
        }

        return profile;
    }

    public static bool TryGet(string name, out IDisplayProfile profile)
    {
        var normalized = NormalizeMapName(name);
        return Profiles.TryGetValue(normalized ?? string.Empty, out profile!);
    }

    public static IGlyphMap GetMap(string name)
    {
        var normalized = NormalizeMapName(name);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("A map name is required.", nameof(name));
        }

        if (Maps.TryGetValue(normalized, out var map))
        {
            return map;
        }

        if (GlyphMapCatalog.BuiltInJson.TryGetValue(normalized, out var json))
        {
            map = CreateMapFromJson(normalized, json);
            Maps[normalized] = map;
            return map;
        }

        throw new KeyNotFoundException($"Glyph map '{normalized}' was not found.");
    }

    public static bool TryGetMap(string name, out IGlyphMap map)
    {
        var normalized = NormalizeMapName(name);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            map = null!;
            return false;
        }

        if (Maps.TryGetValue(normalized, out map))
        {
            return true;
        }

        if (GlyphMapCatalog.BuiltInJson.TryGetValue(normalized, out var json))
        {
            map = CreateMapFromJson(normalized, json);
            Maps[normalized] = map;
            return true;
        }

        map = null!;
        return false;
    }

    public static bool IsCharacterEnabled(string profileName, char c)
    {
        var normalized = NormalizeMapName(profileName);
        if (Maps.TryGetValue(normalized ?? string.Empty, out var map))
        {
            return map.IsSupported(c);
        }

        return TryGet(normalized, out var profile) && profile.IsSupported(c);
    }

    public static void SetCharacterEnabled(string profileName, char c, bool enabled)
    {
        var normalized = NormalizeMapName(profileName);
        if (!TryGet(normalized, out _))
        {
            throw new KeyNotFoundException($"Display profile '{normalized}' was not found.");
        }

        if (!Maps.TryGetValue(normalized ?? string.Empty, out var map))
        {
            return;
        }

        map.SetCharacterEnabled(c, enabled);
    }

    public static bool IsSupported(string profileName, char c)
    {
        var normalized = NormalizeMapName(profileName);
        return TryGet(normalized, out var profile) && IsCharacterEnabled(normalized, c);
    }

    public static bool IsSupported(IDisplayProfile profile, char c)
    {
        return profile is not null && IsCharacterEnabled(profile.Name, c);
    }
}
