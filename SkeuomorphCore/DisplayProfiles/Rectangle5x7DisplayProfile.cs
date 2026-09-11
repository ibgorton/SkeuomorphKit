using System.Collections.Generic;

namespace SkeuomorphCore;

public sealed class Rectangle5x7DisplayProfile : DisplayProfileBase
{
    private static readonly HashSet<char> SupportedCharacters = BuildRectangle5x7Set();

    public Rectangle5x7DisplayProfile() : base("Rectangle5x7", GlyphLibrary.Width * GlyphLibrary.Height, GlyphLibrary.Width, GlyphLibrary.Height, SupportedCharacters)
    {
    }

    public override bool[] GetBits(char c)
    {
        return c.GetBitsRectangle();
    }

    private static HashSet<char> BuildRectangle5x7Set()
    {
        var set = new HashSet<char>
        {
            ' ', '!', '?', '-', '.', ',', '/', '+', '=', '%', ':', ';', '(', ')', '[', ']', '_',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        };

        foreach (var key in GlyphLibrary.PatternKeys)
        {
            set.Add(key);
        }

        return set;
    }
}
