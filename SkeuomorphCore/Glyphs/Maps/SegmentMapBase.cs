using System;
using System.Collections.Generic;
using System.Linq;

namespace SkeuomorphCore;

public interface IGlyphMap
{
    string Name { get; }
    int MapSegmentCount { get; }
    bool IsSupported(char c);
    bool TryGetMask(char c, out ulong mask);
    bool[] GetBits(char c);
    IReadOnlyDictionary<char, ulong?> Masks { get; }
    bool SetCharacterEnabled(char c, bool enabled);
}

public abstract class SegmentMapBase : IGlyphMap
{
    public static IReadOnlySet<char> PrintableAsciiSet => PrintableAscii.Characters;

    public abstract string MapName { get; }
    public abstract int MapSegmentCount { get; }
    public virtual IReadOnlyCollection<char> MapSupportedCharacters => GetSupportedCharacters();
    public virtual IReadOnlySet<char> MapDisabledCharacters => Masks
        .Where(static pair => !pair.Value.HasValue)
        .Select(static pair => pair.Key)
        .ToHashSet();
    public abstract IReadOnlyDictionary<char, ulong?> Masks { get; }

    public string Name => MapName;

    public virtual bool IsSupported(char c)
    {
        return Masks.TryGetValue(c, out var value) && value.HasValue;
    }

    public virtual bool SetCharacterEnabled(char c, bool enabled)
    {
        if (Masks is not IDictionary<char, ulong?> dictionary)
        {
            return false;
        }

        if (!enabled)
        {
            dictionary[c] = null;
            return true;
        }

        if (GetDefaultMasks().TryGetValue(c, out var defaultMask))
        {
            dictionary[c] = defaultMask;
            return true;
        }

        dictionary[c] = 0UL;
        return true;
    }

    protected virtual IReadOnlyDictionary<char, ulong?> GetDefaultMasks()
    {
        return Masks;
    }

    protected virtual IReadOnlyCollection<char> GetSupportedCharacters()
    {
        return PrintableAsciiSet
            .Where(character => Masks.TryGetValue(character, out var value) && value.HasValue)
            .OrderBy(static c => c)
            .ToArray();
    }

    protected bool[] GetBits(ulong mask, int segmentCount)
    {
        var result = new bool[segmentCount];
        for (var i = 0; i < segmentCount; i++)
        {
            result[i] = ((mask >> i) & 1UL) == 1UL;
        }

        return result;
    }

    public virtual bool[] GetBits(char c)
    {
        return TryGetMask(c, out var mask)
            ? GetBits(mask, MapSegmentCount)
            : new bool[MapSegmentCount];
    }

    public virtual bool TryGetMask(char c, out ulong mask)
    {
        if (Masks.TryGetValue(c, out var value) && value.HasValue)
        {
            mask = value.Value;
            return true;
        }

        mask = 0;
        return false;
    }
}
