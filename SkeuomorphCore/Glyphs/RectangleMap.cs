using System.Collections.Generic;

namespace SkeuomorphCore
{
    public static class RectangleMap
    {
        private const int Width = GlyphLibrary.Width;
        private const int Height = GlyphLibrary.Height;
        private const int SegmentCount = Width * Height;

        public static bool[] GetBitsRectangle(this char c)
        {
            if (!DisplayCharacterProfiles.IsSupported("Rectangle5x7", c))
            {
                return new bool[SegmentCount];
            }

            return GlyphLibrary.GetMatrixBits(c);
        }
    }
}