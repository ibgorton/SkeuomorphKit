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
            return GlyphLibrary.GetMatrixBits(c);
        }
    }
}