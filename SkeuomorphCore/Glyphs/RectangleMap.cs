using System.Collections.Generic;

namespace SkeuomorphCore;

public sealed class RectangleMap : SegmentMapBase
{
    public const int Width = GlyphLibrary.Width;
    public const int Height = GlyphLibrary.Height;
    public const int SegmentCount = Width * Height;

    private static readonly IReadOnlyDictionary<char, ulong> RectangleMasks = GlyphLibrary.CreateRectangleMap().Masks;

    public override IReadOnlyDictionary<char, ulong> Masks => RectangleMasks;

    public static IReadOnlyCollection<char> SupportedCharacters => new RectangleMap().GetSupportedCharacters();

    public override bool[] GetBits(char c)
    {
        if (!DisplayCharacterProfiles.IsSupported("Rectangle5x7", c))
        {
            return new bool[SegmentCount];
        }

        return GlyphLibrary.GetMatrixBits(c);
    }
}
