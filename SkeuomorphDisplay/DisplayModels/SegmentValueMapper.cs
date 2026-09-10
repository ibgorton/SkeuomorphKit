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

        public void Apply(ReadOnlySpan<bool> values)
        {
            if (values.Length != _setters.Length)
            {
                throw new ArgumentException($"Expected {_setters.Length} values, received {values.Length}.", nameof(values));
            }

            for (var index = 0; index < values.Length; index++)
            {
                _setters[index](values[index]);
            }
        }

        public void Apply(bool[] values)
        {
            ArgumentNullException.ThrowIfNull(values);
            Apply(values.AsSpan());
        }
    }
}
