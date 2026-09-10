using SkeuomorphCore;

namespace SkeuomorphDisplay
{
    public abstract class SevenSegmentDisplayBase : SegmentDisplayModel
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
            ApplyBitPattern(character.GetBitsSeven());
        }
    }
}
