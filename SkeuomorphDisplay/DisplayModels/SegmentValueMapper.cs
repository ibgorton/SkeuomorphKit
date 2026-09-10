using System;

namespace SkeuomorphDisplay
{
    public sealed class SegmentValueMapper
    {
        private readonly Action<bool>[] _setters;

        public SegmentValueMapper(params Action<bool>[] setters)
        {
            ArgumentNullException.ThrowIfNull(setters);

            if (setters.Length == 0)
            {
                throw new ArgumentException("At least one segment setter is required.", nameof(setters));
            }

            _setters = setters;
        }

        public void Apply(int segmentIndex, bool isOn)
        {
            if (segmentIndex < 0 || segmentIndex >= _setters.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(segmentIndex));
            }

            _setters[segmentIndex](isOn);
        }
    }
}
