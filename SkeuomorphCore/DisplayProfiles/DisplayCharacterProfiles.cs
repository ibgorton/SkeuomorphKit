using System;
using System.Collections.Generic;
using System.Linq;

namespace SkeuomorphCore
{

    public static class DisplayCharacterProfiles
    {
        private static readonly Dictionary<string, IDisplayProfile> Profiles = new(StringComparer.OrdinalIgnoreCase)
        {
            ["SevenSegment"] = new SevenSegmentDisplayProfile(),
            ["Rectangle5x7"] = new Rectangle5x7DisplayProfile(),
            ["SixteenSegment"] = new SixteenSegmentDisplayProfile()
        };

        public static IReadOnlyCollection<string> AllNames => Profiles.Keys.ToArray();

        public static IReadOnlyCollection<IDisplayProfile> AllProfiles => Profiles.Values.ToArray();

        public static void Register(IDisplayProfile profile)
        {
            if (profile is null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            Profiles[profile.Name] = profile;
        }

        public static IDisplayProfile Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("A profile name is required.", nameof(name));
            }

            if (!Profiles.TryGetValue(name, out var profile))
            {
                throw new KeyNotFoundException($"Display profile '{name}' was not found.");
            }

            return profile;
        }

        public static bool TryGet(string name, out IDisplayProfile profile)
        {
            return Profiles.TryGetValue(name ?? string.Empty, out profile!);
        }

        public static bool IsSupported(string profileName, char c)
        {
            return TryGet(profileName, out var profile) && profile.IsSupported(c);
        }

        public static bool IsSupported(IDisplayProfile profile, char c)
        {
            return profile is not null && profile.IsSupported(c);
        }
    }
}
