using SkeuomorphCore;

namespace SkeuomorphDisplay
{
    public abstract class SixteenSegmentDisplayBase : SegmentDisplayModelBase
    {
        protected SixteenSegmentDisplayBase() : base(16)
        {
        }

        public bool ShowDecimalPoint { get; set; }
    }

    public sealed class SixteenSegmentDisplay : SixteenSegmentDisplayBase
    {
        public override void SetChar(char character)
        {
            ShowDecimalPoint = character == '.';
            if (character == '.')
            {
                ApplyBitPattern(new bool[16]);
                return;
            }

            ApplyBitPattern(char.ToUpperInvariant(character).GetBitsSixteen());
        }
    }
}
