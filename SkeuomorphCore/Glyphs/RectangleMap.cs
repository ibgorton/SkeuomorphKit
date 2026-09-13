namespace SkeuomorphCore;

public abstract class RectangleMap : SegmentMapBase
{
    public const int Width = GlyphLibrary.Width;
    public const int Height = GlyphLibrary.Height;
    public const int SegmentCount = Width * Height;

    public static bool[] GetBitsRectangle(char c)
    {
        if (!DisplayCharacterProfiles.IsSupported("Rectangle5x7", c))
        {
            return new bool[SegmentCount];
        }

        return GlyphLibrary.GetMatrixBits(c);
    }
}

public static class RectangleMapExtensions
{
    public static bool[] GetBitsRectangle(this char c)
    {
        return RectangleMap.GetBitsRectangle(c);
    }
}