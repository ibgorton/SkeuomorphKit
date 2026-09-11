using SkeuomorphCore;

namespace SkeuomorphDisplay
{
    public abstract class Rectangle5x7DisplayBase : SegmentDisplayModelBase
    {
        protected Rectangle5x7DisplayBase() : base(35)
        {
        }
    }

    public sealed class Rectangle5x7Display : Rectangle5x7DisplayBase
    {
        public override void SetChar(char character)
        {
            ApplyBitPattern(char.ToUpperInvariant(character).GetBitsRectangle());
        }
    }
}
