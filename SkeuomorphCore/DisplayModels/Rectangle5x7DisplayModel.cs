namespace SkeuomorphCore;

public sealed class Rectangle5x7DisplayModel : SegmentDisplayModelBase
{
    public Rectangle5x7DisplayModel() : base(GlyphLibrary.Width * GlyphLibrary.Height)
    {
    }

    protected override bool[] GetBits(char character)
    {
        return character.GetBitsRectangle();
    }
}
