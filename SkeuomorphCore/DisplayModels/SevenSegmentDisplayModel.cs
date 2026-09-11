namespace SkeuomorphCore
{
    public sealed class SevenSegmentDisplayModel : SegmentDisplayModelBase
    {
        public SevenSegmentDisplayModel() : base(7)
        {
        }

        protected override bool[] GetBits(char character)
        {
            return character.GetBitsSeven();
        }
    }
}
