namespace SkeuomorphCore;

public sealed class SixteenSegmentDisplayModel : SegmentDisplayModelBase
{
    public SixteenSegmentDisplayModel() : base(16)
    {
    }

    protected override bool[] GetBits(char character)
    {
        return character.GetBitsSixteen();
    }
}
