using System.Collections.Generic;

namespace SkeuomorphCore
{
    public enum DisplayCharacterProfile
    {
        SevenSegment,
        Rectangle5x7,
        SixteenSegment
    }

    public static class DisplayCharacterProfiles
    {
        private static readonly HashSet<char> SevenSegmentSet = new()
        {
            ' ', '-', '.', ':', '=', '_',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'O', 'R', 'S', 'U', 'Y', 'r', 'o'
        };

        private static readonly HashSet<char> Rectangle5x7Set = BuildRectangle5x7Set();
        private static readonly HashSet<char> SixteenSegmentSet = BuildSixteenSegmentSet();

        public static bool IsSupported(DisplayCharacterProfile profile, char c)
        {
            var normalized = char.ToUpperInvariant(c);

            return profile switch
            {
                DisplayCharacterProfile.SevenSegment => SevenSegmentSet.Contains(normalized) || SevenSegmentSet.Contains(c),
                DisplayCharacterProfile.Rectangle5x7 => Rectangle5x7Set.Contains(normalized) || Rectangle5x7Set.Contains(c),
                DisplayCharacterProfile.SixteenSegment => SixteenSegmentSet.Contains(normalized) || SixteenSegmentSet.Contains(c),
                _ => false
            };
        }

        public static bool IsSupportedForSevenSegment(char c) => IsSupported(DisplayCharacterProfile.SevenSegment, c);

        public static bool IsSupportedForRectangle5x7(char c) => IsSupported(DisplayCharacterProfile.Rectangle5x7, c);

        public static bool IsSupportedForSixteenSegment(char c) => IsSupported(DisplayCharacterProfile.SixteenSegment, c);

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

        private static HashSet<char> BuildSixteenSegmentSet()
        {
            var set = new HashSet<char>
            {
                ' ', '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/', ':', ';', '<', '=', '>', '?', '@',
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                '[', '\\', ']', '^', '_', '`',
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                '{', '|', '}'
            };

            return set;
        }
    }
}
