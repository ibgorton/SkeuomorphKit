using System;
using System.Collections.Generic;
using System.Linq;

namespace SkeuomorphCore;

public abstract class SegmentMapBase
{
    public static IReadOnlySet<char> PrintableAsciiSet => PrintableAscii.Characters;

    public abstract IReadOnlyDictionary<char, ulong> Masks { get; }

    protected IReadOnlyCollection<char> GetSupportedCharacters()
    {
        return PrintableAsciiSet
            .Where(character => Masks.ContainsKey(character))
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

    protected bool[] GetBits(char original, int segmentCount)
    {
        var normalized = char.ToUpperInvariant(original);
        return TryGetMask(original, normalized, out var mask)
            ? GetBits(mask, segmentCount)
            : new bool[segmentCount];
    }

    protected bool TryGetMask(char original, char normalized, out ulong mask)
    {
        if (Masks.TryGetValue(original, out mask))
        {
            return true;
        }

        return Masks.TryGetValue(normalized, out mask);
    }

    public abstract bool[] GetBits(char c);
}
