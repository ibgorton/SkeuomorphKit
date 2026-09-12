using SkeuomorphCore;

namespace SkeuomorphDisplay
{
    public abstract class Rectangle5x7DisplayBase : SegmentDisplayState
    {
        protected Rectangle5x7DisplayBase() : base(35)
        {
        }
    }

    public sealed class Rectangle5x7Display : Rectangle5x7DisplayBase
    {
        public override void SetChar(char character)
        {
            if (!TrySetCurrentCharacter(character))
            {
                return;
            }

            ApplyBitPattern(char.ToUpperInvariant(character).GetBitsRectangle());
        }
    }
}
