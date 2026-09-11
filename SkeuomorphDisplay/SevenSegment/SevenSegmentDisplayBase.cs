using SkeuomorphCore;

namespace SkeuomorphDisplay
{
    public abstract class SevenSegmentDisplayBase : SegmentDisplayModelBase
    {
        protected SevenSegmentDisplayBase() : base(7)
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
                ApplyBitPattern(new bool[7]);
                return;
            }

            ApplyBitPattern(character.GetBitsSeven());
        }
    }
}
