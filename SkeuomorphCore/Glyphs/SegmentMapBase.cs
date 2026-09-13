using System.Collections.Generic;

namespace SkeuomorphCore;

public abstract class SegmentMapBase
{
    protected static bool[] ToBits(byte mask, int segmentCount)
    {
        var result = new bool[segmentCount];
        for (var i = 0; i < segmentCount; i++)
        {
            result[i] = ((mask >> i) & 1) == 1;
        }

        return result;
    }

    protected static bool[] ToBits(ushort mask, int segmentCount)
    {
        var result = new bool[segmentCount];
        for (var i = 0; i < segmentCount; i++)
        {
            result[i] = ((mask >> i) & 1) == 1;
        }

        return result;
    }

    protected static bool TryGetMask<TMask>(IReadOnlyDictionary<char, TMask> masks, char original, char normalized, out TMask mask)
    {
        if (masks.TryGetValue(original, out mask))
        {
            return true;
        }

        return masks.TryGetValue(normalized, out mask);
    }
}
