using System.Collections.Generic;

namespace SkeuomorphCore;

public sealed class FourteenSegmentDisplayProfile : DisplayProfileBase
{
    private static readonly HashSet<char> SupportedCharacters = BuildFourteenSegmentSet();

    public FourteenSegmentDisplayProfile() : base("FourteenSegment", 14, 4, 4, SupportedCharacters)
    {
    }

    public override bool[] GetBits(char c)
    {
        return c.GetBitsFourteen();
    }

    private static HashSet<char> BuildFourteenSegmentSet()
    {
        return new HashSet<char>
        {
            ' ', '-', '.', ':', '=', '_',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '/', '?', '@', '[', '\\', ']', '^', '`', '{', '|', '}', '~'
        };
    }
}
