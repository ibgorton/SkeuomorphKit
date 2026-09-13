using System.Collections.Generic;
using System.Linq;

namespace SkeuomorphCore;

public abstract class BitmapGlyphSourceBase : IBitmapGlyphSource
{
    protected BitmapGlyphSourceBase(string name, IReadOnlyDictionary<char, BitmapGlyph> glyphs)
    {
        Name = name;
        Glyphs = glyphs;
        Width = 8;
        Height = 8;
    }

    public string Name { get; }
    public int Width { get; }
    public int Height { get; }
    public IReadOnlyCollection<char> SupportedCharacters => Glyphs.Keys.ToArray();

    protected IReadOnlyDictionary<char, BitmapGlyph> Glyphs { get; }

    public bool TryGetGlyph(char c, out BitmapGlyph glyph)
    {
        return Glyphs.TryGetValue(c, out glyph!);
    }
}
