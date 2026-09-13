using System.Collections.Generic;
using System.Linq;

namespace SkeuomorphCore;

public abstract class BitmapGlyphMapBase : IBitmapGlyphMap
{
    public abstract string MapName { get; }

    public string Name => MapName;

    public abstract int Width { get; }

    public abstract int Height { get; }

    public abstract IReadOnlyDictionary<char, BitmapGlyph?> RuntimeGlyphs { get; }

    public abstract IReadOnlyDictionary<char, BitmapGlyph?> DefaultGlyphs { get; }

    public virtual IReadOnlyCollection<char> SupportedCharacters => RuntimeGlyphs
        .Where(static pair => pair.Value is not null)
        .Select(static pair => pair.Key)
        .OrderBy(static c => c)
        .ToArray();

    public virtual bool IsSupported(char c)
    {
        return TryGetGlyph(c, out _);
    }

    public virtual bool TryGetGlyph(char c, out BitmapGlyph glyph)
    {
        if (RuntimeGlyphs.TryGetValue(c, out var value) && value is not null)
        {
            glyph = value;
            return true;
        }

        glyph = default!;
        return false;
    }

    public virtual bool SetCharacterEnabled(char c, bool enabled)
    {
        if (RuntimeGlyphs is not IDictionary<char, BitmapGlyph?> dictionary)
        {
            return false;
        }

        if (!enabled)
        {
            dictionary[c] = null;
            return true;
        }

        if (DefaultGlyphs.TryGetValue(c, out var defaultGlyph) && defaultGlyph is not null)
        {
            dictionary[c] = defaultGlyph;
            return true;
        }

        dictionary[c] = null;
        return true;
    }
}
