using System.Collections.Generic;

namespace SkeuomorphCore;

public sealed class RectangleMap : SegmentMapBase
{
    public const int Width = GlyphLibrary.Width;
    public const int Height = GlyphLibrary.Height;
    public const int SegmentCount = Width * Height;

    public override string MapName => nameof(RectangleMap);
    public override int MapSegmentCount => SegmentCount;
    public override IReadOnlyCollection<char> MapSupportedCharacters => SupportedCharacters;

    private static readonly Dictionary<char, ulong?> DefaultMasks = new();

    static RectangleMap()
    {
        foreach (var pair in GlyphLibrary.CreateRectangleMap().Masks)
        {
            DefaultMasks[pair.Key] = pair.Value;
        }

        RuntimeMasks = new Dictionary<char, ulong?>(DefaultMasks);
    }

    private static readonly Dictionary<char, ulong?> RuntimeMasks;

    public override IReadOnlyDictionary<char, ulong?> Masks => RuntimeMasks;

    protected override IReadOnlyDictionary<char, ulong?> GetDefaultMasks()
    {
        return DefaultMasks;
    }

    public static IReadOnlyCollection<char> SupportedCharacters => new RectangleMap().GetSupportedCharacters();
}
