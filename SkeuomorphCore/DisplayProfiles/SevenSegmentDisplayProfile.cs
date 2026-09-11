using System.Collections.Generic;

namespace SkeuomorphCore
{
    public sealed class SevenSegmentDisplayProfile : DisplayProfileBase
    {
        private static readonly HashSet<char> SupportedCharacters = BuildSevenSegmentSet();

        public SevenSegmentDisplayProfile() : base("SevenSegment", 7, 7, 1, SupportedCharacters)
        {
        }

        public override bool[] GetBits(char c)
        {
            return c.GetBitsSeven();
        }

        private static HashSet<char> BuildSevenSegmentSet()
        {
            var set = new HashSet<char>
            {
                ' ', '-', '.', ':', '=', '_',
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                'O', 'R', 'S', 'U', 'Y', 'r', 'o'
            };

            return set;
        }
    }
}
