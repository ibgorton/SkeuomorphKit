using System;
using System.Collections.Generic;

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

    public readonly record struct SegmentDisplaySnapshot(int SegmentCount, bool[] Segments);

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

        public Action<SegmentDisplaySnapshot>? StateChangedCallback { get; set; }

        public double IncrementFactor { get; set; }

        public int SegmentCount => _segments.Length;

        public bool GetSegmentState(int segmentIndex)
        {
            ValidateSegmentIndex(segmentIndex);
            lock (_syncRoot)
            {
                return _segments[segmentIndex];
            }
        }

        public bool[] GetSegments()
        {
            lock (_syncRoot)
            {
                return (bool[])_segments.Clone();
            }
        }

        public SegmentDisplaySnapshot GetSnapshot()
        {
            lock (_syncRoot)
            {
                return new SegmentDisplaySnapshot(_segments.Length, (bool[])_segments.Clone());
            }
        }

        public void CopySegments(Span<bool> destination)
        {
            if (destination.Length < _segments.Length)
            {
                throw new ArgumentException($"Destination span must hold at least {SegmentCount} entries.", nameof(destination));
            }

            lock (_syncRoot)
            {
                _segments.AsSpan().CopyTo(destination);
            }
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

            var snapshot = GetSnapshot();
            StateChangedCallback?.Invoke(snapshot);
            SegmentStateChanged?.Invoke(this, new SegmentStateChangedEventArgs(segmentIndex, isOn));
        }

        public void ApplyBitPattern(bool[] source)
        {
            ArgumentNullException.ThrowIfNull(source);
            ApplyBitPattern(source.AsSpan());
        }

        public void ApplyBitPattern(ReadOnlySpan<bool> source)
        {
            if (source.Length != _segments.Length)
            {
                throw new ArgumentException($"Expected {SegmentCount} segments, received {source.Length}.", nameof(source));
            }

            var changed = new List<int>();

            lock (_syncRoot)
            {
                for (var i = 0; i < source.Length; i++)
                {
                    if (_segments[i] != source[i])
                    {
                        _segments[i] = source[i];
                        changed.Add(i);
                    }
                }
            }

            if (changed.Count > 0)
            {
                var snapshot = GetSnapshot();
                StateChangedCallback?.Invoke(snapshot);
                foreach (var segmentIndex in changed)
                {
                    SegmentStateChanged?.Invoke(this, new SegmentStateChangedEventArgs(segmentIndex, source[segmentIndex]));
                }
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
