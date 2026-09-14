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
    public int Columns { get; set; }

    public int Rows { get; set; }

    public double CanvasWidth { get; set; } = 220;

    public double CanvasHeight { get; set; } = 260;

    public double OffsetX { get; set; } = 22;

    public double OffsetY { get; set; } = 18;

    public double Scale { get; set; } = 16;

    public List<List<double[]>> Segments { get; set; } = new();
}

public sealed class GlyphMapDefinition
{
    public const int CurrentVersion = 1;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    public int Version { get; set; } = CurrentVersion;

    public string Name { get; set; } = string.Empty;

    public GlyphMapKind Kind { get; set; } = GlyphMapKind.Segmented;

    public int SegmentCount { get; set; }

    public List<int> BitOrder { get; set; } = [];

    public GlyphMapLayoutDefinition? Layout { get; set; }

    public Dictionary<string, string> Characters { get; set; } = new(StringComparer.Ordinal);

    public void Normalize()
    {
        if (Version <= 0)
        {
            Version = CurrentVersion;
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException("A glyph map name is required.");
        }

        if (SegmentCount <= 0)
        {
            throw new InvalidOperationException($"The glyph map '{Name}' must define a positive SegmentCount.");
        }

        if (BitOrder.Count == 0)
        {
            BitOrder = CreateDefaultBitOrder(SegmentCount);
        }
        else
        {
            ValidateBitOrder(BitOrder, SegmentCount, Name);
        }

        if (Layout is not null)
        {
            ValidateLayout(Layout, SegmentCount, Name);
        }

        foreach (var pair in Characters)
        {
            if (string.IsNullOrEmpty(pair.Key) || pair.Key.Length != 1)
            {
                throw new InvalidOperationException($"Character key '{pair.Key}' in glyph map '{Name}' must be a single printable character.");
            }

            if (Kind == GlyphMapKind.Bitmap)
            {
                _ = ParseBitmap(pair.Value);
            }
            else
            {
                _ = ParseMask(pair.Value);
            }
        }
    }

    public string ToJson()
    {
        Normalize();
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

    public GlyphMapDefinition RemapBitOrder(IReadOnlyList<int> newBitOrder)
    {
        if (newBitOrder is null)
        {
            throw new ArgumentNullException(nameof(newBitOrder));
        }

        var validatedOrder = ValidateBitOrder(newBitOrder, SegmentCount, Name);
        var remapped = new GlyphMapDefinition
        {
            Version = CurrentVersion,
            Name = Name,
            Kind = Kind,
            SegmentCount = SegmentCount,
            BitOrder = new List<int>(validatedOrder),
            Layout = Layout is null ? null : CloneLayout(Layout),
            Characters = new Dictionary<string, string>(StringComparer.Ordinal)
        };

        foreach (var pair in Characters)
        {
            remapped.Characters[pair.Key] = Kind == GlyphMapKind.Bitmap
                ? pair.Value
                : RemapMaskValue(pair.Value, validatedOrder, SegmentCount);
        }

        remapped.Normalize();
        return remapped;
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

        if (definition.Version <= 0)
        {
            definition.Version = CurrentVersion;
        }

        if (definition.BitOrder.Count == 0)
        {
            definition.BitOrder = CreateDefaultBitOrder(definition.SegmentCount > 0 ? definition.SegmentCount : 1);
        }

        definition.Normalize();
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
                Version = CurrentVersion,
                Name = name,
                Kind = GlyphMapKind.Segmented,
                SegmentCount = segmented.MapSegmentCount,
                BitOrder = CreateDefaultBitOrder(segmented.MapSegmentCount),
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
                Version = CurrentVersion,
                Name = name,
                Kind = GlyphMapKind.Bitmap,
                SegmentCount = bitmapBase.Width * bitmapBase.Height,
                BitOrder = CreateDefaultBitOrder(bitmapBase.Width * bitmapBase.Height),
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

    private static List<int> CreateDefaultBitOrder(int segmentCount)
    {
        return Enumerable.Range(0, Math.Max(1, segmentCount)).ToList();
    }

    private static List<int> ValidateBitOrder(IReadOnlyList<int> bitOrder, int segmentCount, string mapName)
    {
        if (bitOrder.Count == 0)
        {
            return CreateDefaultBitOrder(segmentCount);
        }

        if (bitOrder.Count != segmentCount)
        {
            throw new InvalidOperationException($"Glyph map '{mapName}' bit order length must match SegmentCount ({segmentCount}), but was {bitOrder.Count}.");
        }

        var set = new HashSet<int>();
        for (var i = 0; i < bitOrder.Count; i++)
        {
            var value = bitOrder[i];
            if (value < 0 || value >= segmentCount)
            {
                throw new InvalidOperationException($"Glyph map '{mapName}' bit order contains an out-of-range index '{value}' for SegmentCount {segmentCount}.");
            }

            if (!set.Add(value))
            {
                throw new InvalidOperationException($"Glyph map '{mapName}' bit order must be a unique permutation of 0..{segmentCount - 1}.");
            }
        }

        return bitOrder.ToList();
    }

    private static void ValidateLayout(GlyphMapLayoutDefinition layout, int segmentCount, string mapName)
    {
        if (layout is null)
        {
            return;
        }

        if (layout.Segments.Count > 0 && layout.Segments.Count != segmentCount)
        {
            throw new InvalidOperationException($"Glyph map '{mapName}' layout segment count must match SegmentCount ({segmentCount}), but was {layout.Segments.Count}.");
        }

        for (var i = 0; i < layout.Segments.Count; i++)
        {
            var segment = layout.Segments[i];
            if (segment is null || segment.Count == 0)
            {
                throw new InvalidOperationException($"Glyph map '{mapName}' segment {i} is missing points.");
            }

            foreach (var point in segment)
            {
                if (point is null || point.Length != 2)
                {
                    throw new InvalidOperationException($"Glyph map '{mapName}' segment {i} has an invalid point definition.");
                }
            }
        }
    }

    private static GlyphMapLayoutDefinition CloneLayout(GlyphMapLayoutDefinition source)
    {
        return new GlyphMapLayoutDefinition
        {
            Columns = source.Columns,
            Rows = source.Rows,
            CanvasWidth = source.CanvasWidth,
            CanvasHeight = source.CanvasHeight,
            OffsetX = source.OffsetX,
            OffsetY = source.OffsetY,
            Scale = source.Scale,
            Segments = source.Segments
                .Select(static segment => segment.Select(static point => point.ToArray()).ToList())
                .ToList()
        };
    }

    private static string RemapMaskValue(string value, IReadOnlyList<int> bitOrder, int segmentCount)
    {
        var mask = ParseMask(value);
        if (!mask.HasValue)
        {
            return "null";
        }

        ulong remapped = 0;
        for (var canonicalIndex = 0; canonicalIndex < bitOrder.Count; canonicalIndex++)
        {
            var targetIndex = bitOrder[canonicalIndex];
            if (targetIndex < 0 || targetIndex >= segmentCount)
            {
                throw new InvalidOperationException($"Bit order entry {targetIndex} is out of range for SegmentCount {segmentCount}.");
            }

            if ((mask.Value & (1UL << canonicalIndex)) != 0)
            {
                remapped |= 1UL << targetIndex;
            }
        }

        return $"0x{remapped:X}";
    }
}

public static class GlyphMapCatalog
{
    private static readonly Lazy<IReadOnlyDictionary<string, string>> LazyBuiltInJson = new(LoadBuiltInJson);
    private static readonly Lazy<IReadOnlyDictionary<string, GlyphMapDefinition>> LazyBuiltInDefinitions = new(LoadBuiltInDefinitions);

    public static IReadOnlyDictionary<string, string> BuiltInJson => LazyBuiltInJson.Value;

    public static IReadOnlyDictionary<string, GlyphMapDefinition> BuiltInDefinitions => LazyBuiltInDefinitions.Value;

    public static IReadOnlyDictionary<string, GlyphMapDefinition> GetBuiltInDefinitions()
    {
        return BuiltInDefinitions;
    }

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

    public static string SerializeForWeb()
    {
        return JsonSerializer.Serialize(
            BuiltInDefinitions
                .OrderBy(static pair => pair.Key, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(static pair => pair.Key, static pair => pair.Value, StringComparer.OrdinalIgnoreCase),
            new JsonSerializerOptions { WriteIndented = true });
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

    private static IReadOnlyDictionary<string, GlyphMapDefinition> LoadBuiltInDefinitions()
    {
        var result = new Dictionary<string, GlyphMapDefinition>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in BuiltInJson)
        {
            result[pair.Key] = GlyphMapDefinition.FromJson(pair.Value, pair.Key);
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
