using System;

namespace SkeuomorphDisplay
{
    public static class DisplaySegmentState
    {
        public static void ApplyBitPattern(ReadOnlySpan<bool> source, Action<int, bool> applySegment)
        {
            ArgumentNullException.ThrowIfNull(applySegment);

            for (var index = 0; index < source.Length; index++)
            {
                applySegment(index, source[index]);
            }
        }

        public static void ApplyBitPattern(bool[] source, Action<int, bool> applySegment)
        {
            ArgumentNullException.ThrowIfNull(source);
            ApplyBitPattern(source.AsSpan(), applySegment);
        }

        public static void ApplyBlank(int segmentCount, Action<int, bool> applySegment)
        {
            ApplyBitPattern(new bool[segmentCount], applySegment);
        }
    }
}
