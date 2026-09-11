using System;

namespace SkeuomorphCore
{
    public abstract class SegmentDisplayModelBase
    {
        protected SegmentDisplayModelBase(int segmentCount)
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
}
