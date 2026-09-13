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

    public static bool[] GetBits(this char c, string profileName)
    {
        if (string.IsNullOrWhiteSpace(profileName))
        {
            throw new ArgumentException("A display profile name is required.", nameof(profileName));
        }

        return DisplayCharacterProfiles.Get(profileName).GetBits(c);
    }
}
