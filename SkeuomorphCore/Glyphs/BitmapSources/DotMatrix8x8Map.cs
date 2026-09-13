#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SkeuomorphCore;

public interface IBitmapGlyphSource
{
    string Name { get; }
    int Width { get; }
    int Height { get; }
    IReadOnlyCollection<char> SupportedCharacters { get; }
    bool TryGetGlyph(char c, out BitmapGlyph glyph);
}

public sealed record BitmapGlyph(int Width, int Height, byte[] Rows)
{
    public bool HasPixel(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
        {
            return false;
        }

        var row = Rows[y];
        return ((row >> (Width - 1 - x)) & 1) == 1;
    }

    public BitmapGlyph NormalizeTo(int targetWidth, int targetHeight)
    {
        var normalized = new byte[targetHeight];
        var xOffset = Math.Max(0, (targetWidth - Width) / 2);
        var yOffset = Math.Max(0, (targetHeight - Height) / 2);

        for (var row = 0; row < Height; row++)
        {
            for (var col = 0; col < Width; col++)
            {
                if (!HasPixel(col, row))
                {
                    continue;
                }

                var targetX = col + xOffset;
                var targetY = row + yOffset;
                if (targetX < 0 || targetX >= targetWidth || targetY < 0 || targetY >= targetHeight)
                {
                    continue;
                }

                normalized[targetY] |= (byte)(1 << (targetWidth - 1 - targetX));
            }
        }

        return new BitmapGlyph(targetWidth, targetHeight, normalized);
    }

    public ulong ToMask(int targetWidth, int targetHeight)
    {
        var normalized = NormalizeTo(targetWidth, targetHeight);
        ulong mask = 0;
        for (var row = 0; row < normalized.Height; row++)
        {
            for (var col = 0; col < normalized.Width; col++)
            {
                if (normalized.HasPixel(col, row))
                {
                    mask |= 1UL << (row * targetWidth + col);
                }
            }
        }

        return mask;
    }
}

public sealed class SimpleBitmapGlyphSource : IBitmapGlyphSource
{
    private readonly Dictionary<char, BitmapGlyph> _glyphs;

    public SimpleBitmapGlyphSource(string name, Dictionary<char, BitmapGlyph> glyphs)
    {
        Name = name;
        _glyphs = new Dictionary<char, BitmapGlyph>(glyphs);
        Width = _glyphs.Count == 0 ? 0 : _glyphs.First().Value.Width;
        Height = _glyphs.Count == 0 ? 0 : _glyphs.First().Value.Height;
    }

    public string Name { get; }
    public int Width { get; }
    public int Height { get; }
    public IReadOnlyCollection<char> SupportedCharacters => _glyphs.Keys;

    public bool TryGetGlyph(char c, out BitmapGlyph glyph)
    {
        return _glyphs.TryGetValue(c, out glyph!);
    }
}

public sealed class BdfBitmapFontSource : IBitmapGlyphSource
{
    private readonly Dictionary<char, BitmapGlyph> _glyphs;

    public BdfBitmapFontSource(string name, int width, int height, Dictionary<char, BitmapGlyph> glyphs)
    {
        Name = name;
        Width = width;
        Height = height;
        _glyphs = glyphs;
    }

    public string Name { get; }
    public int Width { get; }
    public int Height { get; }
    public IReadOnlyCollection<char> SupportedCharacters => _glyphs.Keys;

    public bool TryGetGlyph(char c, out BitmapGlyph glyph)
    {
        return _glyphs.TryGetValue(c, out glyph!);
    }
}

public static class BdfBitmapFontImporter
{
    public static BdfBitmapFontSource Parse(string name, string sourceText, int targetWidth = 8, int targetHeight = 8)
    {
        using var reader = new StringReader(sourceText);
        return Parse(name, reader, targetWidth, targetHeight);
    }

    public static BdfBitmapFontSource Parse(string name, TextReader reader, int targetWidth = 8, int targetHeight = 8)
    {
        var glyphs = new Dictionary<char, BitmapGlyph>();
        string? startChar = null;
        int? encoding = null;
        string? bbx = null;
        var bitmap = new List<byte>();

        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            line = line.Trim();
            if (line.StartsWith("STARTCHAR ", StringComparison.Ordinal))
            {
                startChar = line[10..];
            }
            else if (line.StartsWith("ENCODING ", StringComparison.Ordinal))
            {
                var value = line[9..].Trim();
                if (int.TryParse(value, out var parsed))
                {
                    encoding = parsed;
                }
            }
            else if (line.StartsWith("BBX", StringComparison.Ordinal))
            {
                bbx = line[3..].Trim();
            }
            else if (line == "BITMAP")
            {
                bitmap = new List<byte>();
            }
            else if (line == "ENDCHAR")
            {
                if (encoding is not null && startChar is not null && bbx is not null && bitmap.Count > 0)
                {
                    var glyph = NormalizeGlyph(bbx, bitmap, targetWidth, targetHeight);
                    glyphs[(char)encoding.Value] = glyph;
                }

                startChar = null;
                encoding = null;
                bbx = null;
                bitmap = new List<byte>();
            }
            else if (bitmap.Count > 0 && line.Length == 2)
            {
                if (byte.TryParse(line, System.Globalization.NumberStyles.HexNumber, null, out var value))
                {
                    bitmap.Add(value);
                }
            }
        }

        return new BdfBitmapFontSource(name, targetWidth, targetHeight, glyphs);
    }

    private static BitmapGlyph NormalizeGlyph(string bbx, List<byte> rows, int targetWidth, int targetHeight)
    {
        var parts = bbx.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length < 4)
        {
            return new BitmapGlyph(targetWidth, targetHeight, new byte[targetHeight]);
        }

        var sourceWidth = int.Parse(parts[0]);
        var sourceHeight = int.Parse(parts[1]);
        var xOffset = int.Parse(parts[2]);
        var yOffset = int.Parse(parts[3]);

        var normalized = new byte[targetHeight];
        for (var row = 0; row < Math.Min(sourceHeight, rows.Count); row++)
        {
            var sourceRow = rows[row];
            for (var col = 0; col < sourceWidth; col++)
            {
                var bit = (sourceRow >> (8 - 1 - col)) & 1;
                if (bit == 0)
                {
                    continue;
                }

                var targetX = col + xOffset;
                var targetY = row + yOffset;
                if (targetX < 0 || targetY < 0 || targetX >= targetWidth || targetY >= targetHeight)
                {
                    continue;
                }

                normalized[targetY] |= (byte)(1 << (targetWidth - 1 - targetX));
            }
        }

        return new BitmapGlyph(targetWidth, targetHeight, normalized);
    }
}

public static class DotMatrix8x8GlyphStyles
{
    public const string Default = "default";
    public const string Serif = "serif";
    public const string SansSerif = "sans-serif";
    public const string Classic = "classic";
    public const string Alternate = "ic8x8u";
    public const string Compact = "icl8x8u";

    public static IReadOnlyCollection<string> All => new[] { Default, Serif, SansSerif, Classic, Alternate, Compact };

    public static string Normalize(string style)
    {
        if (string.IsNullOrWhiteSpace(style))
        {
            return Default;
        }

        var normalized = style.Trim();

        if (normalized.Equals("default", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("serif", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("sans-serif", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("sans serif", StringComparison.OrdinalIgnoreCase))
        {
            return Default;
        }

        if (normalized.Equals("classic", StringComparison.OrdinalIgnoreCase) || normalized.Equals("ib8x8u", StringComparison.OrdinalIgnoreCase))
        {
            return Classic;
        }

        if (normalized.Equals("ic8x8u", StringComparison.OrdinalIgnoreCase) || normalized.Equals("alt", StringComparison.OrdinalIgnoreCase))
        {
            return Alternate;
        }

        if (normalized.Equals("icl8x8u", StringComparison.OrdinalIgnoreCase) || normalized.Equals("compact", StringComparison.OrdinalIgnoreCase))
        {
            return Compact;
        }

        return normalized;
    }
}

public sealed class DotMatrix8x8Map : SegmentMapBase
{
    public const int Width = 8;
    public const int Height = 8;
    public const int SegmentCount = Width * Height;

    private static readonly Dictionary<string, IBitmapGlyphSource> Sources = CreateSources();
    private static readonly Dictionary<char, ulong?> DefaultMasks = BuildRuntimeMasks();
    private static readonly Dictionary<char, ulong?> RuntimeMasks = new(DefaultMasks);
    private static readonly Dictionary<char, BitmapGlyph?> DefaultGlyphs = BuildDefaultGlyphs();
    private static readonly Dictionary<char, BitmapGlyph?> RuntimeGlyphs = new(DefaultGlyphs);

    public override string MapName => nameof(DotMatrix8x8Map);
    public override int MapSegmentCount => SegmentCount;
    public override IReadOnlyCollection<char> MapSupportedCharacters => SupportedCharacters;
    public override IReadOnlyDictionary<char, ulong?> Masks => RuntimeMasks;

    protected override IReadOnlyDictionary<char, ulong?> GetDefaultMasks()
    {
        return DefaultMasks;
    }

    public static IReadOnlyCollection<string> SupportedStyles => Sources.Keys;
    public static IReadOnlyCollection<char> SupportedCharacters => new DotMatrix8x8Map().GetSupportedCharacters();
    public static IReadOnlyDictionary<char, BitmapGlyph?> DefaultGlyphsMap => DefaultGlyphs;
    public static IReadOnlyDictionary<char, BitmapGlyph?> RuntimeGlyphsMap => RuntimeGlyphs;

    protected override IReadOnlyCollection<char> GetSupportedCharacters()
    {
        return BuildExtendedAsciiCharacterSet()
            .Where(character => Masks.TryGetValue(character, out var value) && value.HasValue)
            .OrderBy(static c => c)
            .ToArray();
    }

    public static bool TryGetMask(char c, string style, out ulong mask)
    {
        var normalizedStyle = DotMatrix8x8GlyphStyles.Normalize(style);
        if (!Sources.TryGetValue(normalizedStyle, out var source) || !source.TryGetGlyph(c, out var glyph))
        {
            mask = 0;
            return false;
        }

        mask = glyph.ToMask(Width, Height);
        return true;
    }

    public static bool[] GetBits(char c, string style = DotMatrix8x8GlyphStyles.Default)
    {
        if (!TryGetMask(c, style, out var mask))
        {
            return new bool[SegmentCount];
        }

        var bits = new bool[SegmentCount];
        for (var index = 0; index < SegmentCount; index++)
        {
            bits[index] = ((mask >> index) & 1UL) == 1UL;
        }

        return bits;
    }

    public static bool TryGetPattern(char c, string style, out string[] rows)
    {
        if (!TryGetMask(c, style, out var mask))
        {
            rows = Array.Empty<string>();
            return false;
        }

        rows = DecodeMask(mask);
        return true;
    }

    private static Dictionary<string, IBitmapGlyphSource> CreateSources()
    {
        var defaultSource = BuildDefaultSource();
        var classicSource = BuildBitmapSource(BitmapGlyphSources.IbmBios8x8, DotMatrix8x8GlyphStyles.Classic);
        var alternateSource = BuildBitmapSource(BitmapGlyphSources.IbmCga8x8, DotMatrix8x8GlyphStyles.Alternate);
        var compactSource = BuildBitmapSource(BitmapGlyphSources.IbmCgaLight8x8, DotMatrix8x8GlyphStyles.Compact);
        return new Dictionary<string, IBitmapGlyphSource>(StringComparer.OrdinalIgnoreCase)
        {
            [DotMatrix8x8GlyphStyles.Default] = defaultSource,
            [DotMatrix8x8GlyphStyles.Serif] = defaultSource,
            [DotMatrix8x8GlyphStyles.SansSerif] = defaultSource,
            [DotMatrix8x8GlyphStyles.Classic] = classicSource,
            [DotMatrix8x8GlyphStyles.Alternate] = alternateSource,
            [DotMatrix8x8GlyphStyles.Compact] = compactSource,
        };
    }

    private static Dictionary<char, ulong?> BuildRuntimeMasks()
    {
        var masks = new Dictionary<char, ulong?>();
        foreach (var character in BuildExtendedAsciiCharacterSet())
        {
            if (TryGetMask(character, DotMatrix8x8GlyphStyles.Default, out var mask))
            {
                masks[character] = mask;
            }
            else
            {
                masks[character] = null;
            }
        }

        return masks;
    }

    private static Dictionary<char, BitmapGlyph?> BuildDefaultGlyphs()
    {
        var glyphs = new Dictionary<char, BitmapGlyph?>();
        foreach (var character in BuildExtendedAsciiCharacterSet())
        {
            if (TryGetMask(character, DotMatrix8x8GlyphStyles.Default, out var mask))
            {
                glyphs[character] = BitmapGlyphFromMask(mask, Width, Height);
                continue;
            }

            glyphs[character] = null;
        }

        return glyphs;
    }

    private static HashSet<char> BuildExtendedAsciiCharacterSet()
    {
        var set = new HashSet<char>(PrintableAscii.Characters);
        for (var codePoint = 0; codePoint <= 127; codePoint++)
        {
            set.Add((char)codePoint);
        }

        return set;
    }

    private static IBitmapGlyphSource BuildDefaultSource()
    {
        var glyphs = new Dictionary<char, BitmapGlyph>();
        foreach (var character in BuildExtendedAsciiCharacterSet())
        {
            if (GlyphLibrary.TryGetMask(character, out var sourceMask))
            {
                var glyph = BitmapGlyphFromMask(sourceMask, GlyphLibrary.Width, GlyphLibrary.Height);
                glyphs[character] = glyph.NormalizeTo(Width, Height);
                continue;
            }

            foreach (var candidate in new IReadOnlyDictionary<char, BitmapGlyph>[]
                     {
                         BitmapGlyphSources.IbmBios8x8,
                         BitmapGlyphSources.IbmCga8x8,
                         BitmapGlyphSources.IbmCgaLight8x8,
                     })
            {
                if (candidate.TryGetValue(character, out var bitmapGlyph))
                {
                    glyphs[character] = bitmapGlyph;
                    break;
                }
            }
        }

        return new SimpleBitmapGlyphSource(DotMatrix8x8GlyphStyles.Default, glyphs);
    }

    private static IBitmapGlyphSource BuildBitmapSource(IReadOnlyDictionary<char, BitmapGlyph> glyphs, string styleName)
    {
        return new SimpleBitmapGlyphSource(styleName, new Dictionary<char, BitmapGlyph>(glyphs));
    }

    private static BitmapGlyph BitmapGlyphFromMask(ulong sourceMask, int width, int height)
    {
        var rows = new byte[height];
        for (var row = 0; row < height; row++)
        {
            for (var col = 0; col < width; col++)
            {
                if (((sourceMask >> (row * width + col)) & 1UL) == 1UL)
                {
                    rows[row] |= (byte)(1 << (width - 1 - col));
                }
            }
        }

        return new BitmapGlyph(width, height, rows);
    }

    private static string[] DecodeMask(ulong mask)
    {
        var rows = new string[Height];
        for (var row = 0; row < Height; row++)
        {
            var chars = new char[Width];
            for (var col = 0; col < Width; col++)
            {
                chars[col] = ((mask >> (row * Width + col)) & 1UL) == 1UL ? '1' : '0';
            }

            rows[row] = new string(chars);
        }

        return rows;
    }
}

public sealed class DotMatrix8x8BitmapGlyphMap : BitmapGlyphMapBase
{
    public override string MapName => nameof(DotMatrix8x8Map);

    public override int Width => DotMatrix8x8Map.Width;

    public override int Height => DotMatrix8x8Map.Height;

    public override IReadOnlyDictionary<char, BitmapGlyph?> RuntimeGlyphs => DotMatrix8x8Map.RuntimeGlyphsMap;

    public override IReadOnlyDictionary<char, BitmapGlyph?> DefaultGlyphs => DotMatrix8x8Map.DefaultGlyphsMap;
}
