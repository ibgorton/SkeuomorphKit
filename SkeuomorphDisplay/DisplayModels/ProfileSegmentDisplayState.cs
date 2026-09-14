using System;
using System.Linq;

using SkeuomorphCore;

namespace SkeuomorphDisplay
{
    public abstract class ProfileSegmentDisplayState : SegmentDisplayState
    {
        private readonly string _profileName;

        protected ProfileSegmentDisplayState(string profileName)
            : base(ResolveSegmentCount(profileName))
        {
            _profileName = profileName;
        }

        protected string ProfileName => _profileName;

        protected void ApplyProfileCharacter(char character)
        {
            if (!TrySetCurrentCharacter(character))
            {
                return;
            }

            var bits = DisplayCharacterProfiles.Get(_profileName).GetBits(character);
            if (bits.Length != SegmentCount)
            {
                var normalized = new bool[SegmentCount];
                for (var index = 0; index < Math.Min(bits.Length, normalized.Length); index++)
                {
                    normalized[index] = bits[index];
                }

                bits = normalized;
            }

            ApplyBitPattern(bits);
        }

        protected static int ResolveSegmentCount(string profileName)
        {
            if (string.IsNullOrWhiteSpace(profileName))
            {
                throw new ArgumentException("A display profile name is required.", nameof(profileName));
            }

            return DisplayCharacterProfiles.Get(profileName).SegmentCount;
        }
    }
}
