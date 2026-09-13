using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SkeuomorphCore;

public enum GlyphMapKind
{
    Segmented,
    Bitmap
}

public sealed class GlyphMapLayoutDefinition
{
    public double CanvasWidth { get; set; } = 220;

    public double CanvasHeight { get; set; } = 260;

    public double OffsetX { get; set; } = 22;

    public double OffsetY { get; set; } = 18;

    public double Scale { get; set; } = 16;

    public List<List<double[]>> Segments { get; set; } = new();
}

public sealed class GlyphMapDefinition
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    public string Name { get; set; } = string.Empty;

    public GlyphMapKind Kind { get; set; } = GlyphMapKind.Segmented;

    public int SegmentCount { get; set; }

    public GlyphMapLayoutDefinition? Layout { get; set; }

    public Dictionary<string, string> Characters { get; set; } = new(StringComparer.Ordinal);

    public string ToJson()
    {
        return JsonSerializer.Serialize(this, SerializerOptions);
    }

    public IReadOnlyDictionary<char, ulong?> ToMasks()
    {
        var result = new Dictionary<char, ulong?>();
        foreach (var pair in Characters)
        {
            if (pair.Key is null || pair.Key.Length != 1)
            {
                continue;
            }

            result[pair.Key[0]] = ParseMask(pair.Value);
        }

        return result;
    }

    public IReadOnlyDictionary<char, BitmapGlyph?> ToBitmaps()
    {
        var result = new Dictionary<char, BitmapGlyph?>();
        foreach (var pair in Characters)
        {
            if (pair.Key is null || pair.Key.Length != 1)
            {
                continue;
            }

            result[pair.Key[0]] = ParseBitmap(pair.Value);
        }

        return result;
    }

    public static GlyphMapDefinition FromJson(string json, string name = null)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("Glyph map JSON is required.", nameof(json));
        }

        var definition = JsonSerializer.Deserialize<GlyphMapDefinition>(json)
            ?? throw new InvalidOperationException("The glyph map JSON could not be deserialized.");

        if (string.IsNullOrWhiteSpace(definition.Name) && !string.IsNullOrWhiteSpace(name))
        {
            definition.Name = name;
        }

        return definition;
    }

    public static GlyphMapDefinition FromMap(string name, IGlyphMap map)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A glyph map name is required.", nameof(name));
        }

        if (map is null)
        {
            throw new ArgumentNullException(nameof(map));
        }

        if (map is ISegmentedGlyphMap segmented)
        {
            return new GlyphMapDefinition
            {
                Name = name,
                Kind = GlyphMapKind.Segmented,
                SegmentCount = segmented.MapSegmentCount,
                Characters = segmented.Masks
                    .OrderBy(static pair => pair.Key)
                    .ToDictionary(
                        static pair => pair.Key.ToString(),
                        static pair => pair.Value.HasValue ? $"0x{pair.Value.Value:X}" : "null",
                        StringComparer.Ordinal)
            };
        }

        if (map is BitmapGlyphMapBase bitmapBase)
        {
            return new GlyphMapDefinition
            {
                Name = name,
                Kind = GlyphMapKind.Bitmap,
                SegmentCount = bitmapBase.Width * bitmapBase.Height,
                Characters = bitmapBase.RuntimeGlyphs
                    .OrderBy(pair => pair.Key)
                    .ToDictionary(
                        pair => pair.Key.ToString(),
                        pair => pair.Value is null ? "null" : SerializeBitmap(pair.Value),
                        StringComparer.Ordinal)
            };
        }

        throw new NotSupportedException($"The glyph map type '{map.GetType().FullName}' is not supported for JSON serialization.");
    }

    private static ulong? ParseMask(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Equals("null", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var trimmed = value.Trim();
        if (trimmed.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            trimmed = trimmed[2..];
            return Convert.ToUInt64(trimmed, 16);
        }

        if (ulong.TryParse(trimmed, NumberStyles.Integer, CultureInfo.InvariantCulture, out var numericValue))
        {
            return numericValue;
        }

        throw new InvalidOperationException($"Unsupported glyph mask value '{value}'.");
    }

    private static string SerializeBitmap(BitmapGlyph glyph)
    {
        var rows = new string[glyph.Height];
        for (var row = 0; row < glyph.Height; row++)
        {
            var bits = new char[glyph.Width];
            for (var col = 0; col < glyph.Width; col++)
            {
                bits[col] = glyph.HasPixel(col, row) ? '1' : '0';
            }

            rows[row] = new string(bits);
        }

        return string.Join('|', rows);
    }

    private static BitmapGlyph? ParseBitmap(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Equals("null", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var rowStrings = value.Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (rowStrings.Length == 0)
        {
            return null;
        }

        var width = rowStrings[0].Length;
        var height = rowStrings.Length;
        var rows = new byte[height];

        for (var row = 0; row < height; row++)
        {
            var rowBits = rowStrings[row];
            if (rowBits.Length != width)
            {
                throw new InvalidOperationException($"Bitmap glyph row length mismatch for '{value}'.");
            }

            for (var col = 0; col < width; col++)
            {
                if (rowBits[col] == '1')
                {
                    rows[row] |= (byte)(1 << (width - 1 - col));
                }
            }
        }

        return new BitmapGlyph(width, height, rows);
    }
}

public static class GlyphMapCatalog
{
    private static readonly Lazy<IReadOnlyDictionary<string, string>> LazyBuiltInJson = new(LoadBuiltInJson);

    public static IReadOnlyDictionary<string, string> BuiltInJson => LazyBuiltInJson.Value;

    public static string Serialize(string name)
    {
        return BuiltInJson.TryGetValue(name ?? string.Empty, out var json)
            ? json
            : throw new KeyNotFoundException($"Built-in glyph map '{name}' was not found.");
    }

    public static GlyphMapDefinition Load(string name)
    {
        return GlyphMapDefinition.FromJson(Serialize(name), name);
    }

    private static IReadOnlyDictionary<string, string> LoadBuiltInJson()
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var directory in GetCandidateJsonDirectories())
        {
            if (!Directory.Exists(directory))
            {
                continue;
            }

            foreach (var file in Directory.EnumerateFiles(directory, "*.json", SearchOption.TopDirectoryOnly))
            {
                var json = File.ReadAllText(file);
                var definition = GlyphMapDefinition.FromJson(json, Path.GetFileNameWithoutExtension(file));
                result[definition.Name] = definition.ToJson();
            }
        }

        return result;
    }

    private static IEnumerable<string> GetCandidateJsonDirectories()
    {
        var candidates = new List<string>
        {
            Path.Combine(AppContext.BaseDirectory, "Glyphs", "Maps"),
            Path.Combine(AppContext.BaseDirectory, "Maps"),
            Path.Combine(Directory.GetCurrentDirectory(), "SkeuomorphCore", "Glyphs", "Maps"),
            Path.Combine(Directory.GetCurrentDirectory(), "SkeuomorphCore", "Glyphs")
        };

        foreach (var path in candidates)
        {
            yield return path;
        }
    }
}

public sealed class JsonGlyphMap : SegmentMapBase
{
    private readonly Dictionary<char, ulong?> _defaultMasks;
    private readonly Dictionary<char, ulong?> _runtimeMasks;

    public JsonGlyphMap(string mapName, IReadOnlyDictionary<char, ulong?> masks, int segmentCount)
    {
        MapName = mapName;
        MapSegmentCount = segmentCount;
        _defaultMasks = new Dictionary<char, ulong?>(masks);
        _runtimeMasks = new Dictionary<char, ulong?>(_defaultMasks);
    }

    public override string MapName { get; }

    public override int MapSegmentCount { get; }

    public override IReadOnlyDictionary<char, ulong?> Masks => _runtimeMasks;

    protected override IReadOnlyDictionary<char, ulong?> GetDefaultMasks()
    {
        return _defaultMasks;
    }
}

public sealed class JsonBitmapGlyphMap : BitmapGlyphMapBase
{
    private readonly Dictionary<char, BitmapGlyph?> _defaultGlyphs;
    private readonly Dictionary<char, BitmapGlyph?> _runtimeGlyphs;

    public JsonBitmapGlyphMap(string mapName, IReadOnlyDictionary<char, BitmapGlyph?> glyphs, int totalPixels)
    {
        MapName = mapName;
        _defaultGlyphs = new Dictionary<char, BitmapGlyph?>(glyphs);
        _runtimeGlyphs = new Dictionary<char, BitmapGlyph?>(_defaultGlyphs);
        Width = Math.Max(1, (int)Math.Sqrt(totalPixels));
        Height = Math.Max(1, totalPixels / Width);
    }

    public override string MapName { get; }

    public override int Width { get; }

    public override int Height { get; }

    public override IReadOnlyDictionary<char, BitmapGlyph?> RuntimeGlyphs => _runtimeGlyphs;

    public override IReadOnlyDictionary<char, BitmapGlyph?> DefaultGlyphs => _defaultGlyphs;
}
