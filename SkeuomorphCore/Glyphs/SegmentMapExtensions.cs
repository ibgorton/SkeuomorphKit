using System;

namespace SkeuomorphCore;

public static class SegmentMapExtensions
{
    public static bool[] GetBits(this char c, IDisplayProfile profile)
    {
        if (profile is null)
        {
            throw new ArgumentNullException(nameof(profile));
        }

        return profile.GetBits(c);
    }

    public static bool[] GetBits<TMap>(this char c) where TMap : SegmentMapBase, new()
    {
        return new TMap().GetBits(c);
    }
}
