using SkeuomorphCore;

namespace SkeuomorphDisplay
{
    public abstract class SevenSegmentDisplayBase : ProfileSegmentDisplayState
    {
        protected SevenSegmentDisplayBase() : base("SevenSegment")
        {
        }

        public double DisplayAngle { get; set; } = -8d;
        public double DecimalDisplayAngle { get; set; } = 8d;

        public bool ShowDecimalPoint { get; set; }
    }

    public sealed class SevenSegmentDisplay : SevenSegmentDisplayBase
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
