using System;

using SkeuomorphCore;

namespace SkeuomorphDisplay
{
    public sealed class SegmentStateChangedEventArgs : EventArgs
    {
        public SegmentStateChangedEventArgs(int segmentIndex, bool isOn)
        {
            SegmentIndex = segmentIndex;
            IsOn = isOn;
        }

        public int SegmentIndex { get; }

        public bool IsOn { get; }
    }

    public abstract class SegmentDisplayModel : IDisplayControl
    {
        private readonly bool[] _segments;
        private readonly object _syncRoot = new();

        protected SegmentDisplayModel(int segmentCount)
        {
            if (segmentCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(segmentCount));
            }

            _segments = new bool[segmentCount];
        }

        public event EventHandler<SegmentStateChangedEventArgs>? SegmentStateChanged;

        public double IncrementFactor { get; set; }

        public int SegmentCount => _segments.Length;

        public bool GetSegmentState(int segmentIndex)
        {
            ValidateSegmentIndex(segmentIndex);
            return _segments[segmentIndex];
        }

        public void SetSegmentState(int segmentIndex, bool isOn)
        {
            ValidateSegmentIndex(segmentIndex);

            lock (_syncRoot)
            {
                if (_segments[segmentIndex] == isOn)
                {
                    return;
                }

                _segments[segmentIndex] = isOn;
            }

            SegmentStateChanged?.Invoke(this, new SegmentStateChangedEventArgs(segmentIndex, isOn));
        }

        public void ApplyBitPattern(bool[] source)
        {
            ArgumentNullException.ThrowIfNull(source);

            if (source.Length != _segments.Length)
            {
                throw new ArgumentException($"Expected {SegmentCount} segments, received {source.Length}.", nameof(source));
            }

            for (var i = 0; i < source.Length; i++)
            {
                SetSegmentState(i, source[i]);
            }
        }

        public void BlankModule()
        {
            ApplyBitPattern(new bool[_segments.Length]);
        }

        public virtual void SetColorBrightness()
        {
        }

        public abstract void SetChar(char character);

        protected void ValidateSegmentIndex(int segmentIndex)
        {
            if (segmentIndex < 0 || segmentIndex >= _segments.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(segmentIndex));
            }
        }
    }

}
