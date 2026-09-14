using SkeuomorphCore;

namespace SkeuomorphDisplay
{
    public abstract class SixteenSegmentDisplayBase : ProfileSegmentDisplayState
    {
        protected SixteenSegmentDisplayBase() : base("SixteenSegment")
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
                ApplyBitPattern(new bool[SegmentCount]);
                return;
            }

            ApplyProfileCharacter(character);
        }
    }
}
