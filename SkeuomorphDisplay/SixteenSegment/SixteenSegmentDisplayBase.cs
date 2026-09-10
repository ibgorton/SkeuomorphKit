using SkeuomorphCore;

namespace SkeuomorphDisplay
{
    public abstract class SixteenSegmentDisplayBase : SegmentDisplayModel
    {
        protected SixteenSegmentDisplayBase() : base(16)
        {
        }
    }

    public sealed class SixteenSegmentDisplay : SixteenSegmentDisplayBase
    {
        public override void SetChar(char character)
        {
            ApplyBitPattern(char.ToUpperInvariant(character).GetBitsSixteen());
        }
    }
}
