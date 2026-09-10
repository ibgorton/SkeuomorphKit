using System;

namespace SkeuomorphCore
{
    public abstract class SegmentDisplayModel
    {
        protected SegmentDisplayModel(int segmentCount)
        {
            SegmentCount = segmentCount;
            Bits = new bool[segmentCount];
        }

        public int SegmentCount { get; }

        public bool[] Bits { get; protected set; }

        public virtual void BlankModule()
        {
            Array.Clear(Bits, 0, Bits.Length);
        }

        public virtual void SetChar(char character)
        {
            var source = GetBits(character);
            if (source.Length != Bits.Length)
            {
                Array.Clear(Bits, 0, Bits.Length);
                return;
            }

            Array.Copy(source, Bits, Bits.Length);
        }

        protected abstract bool[] GetBits(char character);
    }

    public sealed class SevenSegmentDisplayModel : SegmentDisplayModel
    {
        public SevenSegmentDisplayModel() : base(7)
        {
        }

        protected override bool[] GetBits(char character)
        {
            return character.GetBitsSeven();
        }
    }

    public sealed class Rectangle5x7DisplayModel : SegmentDisplayModel
    {
        public Rectangle5x7DisplayModel() : base(GlyphLibrary.Width * GlyphLibrary.Height)
        {
        }

        protected override bool[] GetBits(char character)
        {
            return character.GetBitsRectangle();
        }
    }

    public sealed class SixteenSegmentDisplayModel : SegmentDisplayModel
    {
        public SixteenSegmentDisplayModel() : base(16)
        {
        }

        protected override bool[] GetBits(char character)
        {
            return character.GetBitsSixteen();
        }
    }
}
