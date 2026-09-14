using SkeuomorphCore;

namespace SkeuomorphDisplay
{
    public abstract class Rectangle5x7DisplayBase : ProfileSegmentDisplayState
    {
        protected Rectangle5x7DisplayBase() : base("Rectangle5x7")
        {
        }
    }

    public sealed class Rectangle5x7Display : Rectangle5x7DisplayBase
    {
        public override void SetChar(char character)
        {
            ApplyProfileCharacter(character);
        }
    }
}
