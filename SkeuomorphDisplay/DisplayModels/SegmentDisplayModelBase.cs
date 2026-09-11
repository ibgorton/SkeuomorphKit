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

    public readonly record struct SegmentDisplayUpdate(int SegmentIndex, bool IsOn);

    public abstract class SegmentDisplayModelBase : IDisplayControl
    {
        private readonly bool[] _segments;
        private readonly bool[] _dirtyFlags;
        private readonly object _syncRoot = new();

        protected SegmentDisplayModelBase(int segmentCount)
        {
            if (segmentCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(segmentCount));
            }

            _segments = new bool[segmentCount];
            _dirtyFlags = new bool[segmentCount];
        }

        public event EventHandler<SegmentStateChangedEventArgs>? SegmentStateChanged;

        public bool RaiseSegmentStateChangedEvents { get; set; }

        public int Version { get; private set; }

        public bool HasDirtySegments
        {
            get
            {
                lock (_syncRoot)
                {
                    for (var i = 0; i < _dirtyFlags.Length; i++)
                    {
                        if (_dirtyFlags[i])
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }
        }

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

        public int[] DrainDirtySegments()
        {
            lock (_syncRoot)
            {
                var dirty = new List<int>();
                for (var i = 0; i < _dirtyFlags.Length; i++)
                {
                    if (_dirtyFlags[i])
                    {
                        dirty.Add(i);
                        _dirtyFlags[i] = false;
                    }
                }

                return dirty.ToArray();
            }
        }

        public SegmentDisplayUpdate[] GetDirtyUpdates()
        {
            var dirtySegments = DrainDirtySegments();
            var updates = new SegmentDisplayUpdate[dirtySegments.Length];

            for (var i = 0; i < dirtySegments.Length; i++)
            {
                var segmentIndex = dirtySegments[i];
                updates[i] = new SegmentDisplayUpdate(segmentIndex, GetSegmentState(segmentIndex));
            }

            return updates;
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
                _dirtyFlags[segmentIndex] = true;
                Version++;
            }

            if (RaiseSegmentStateChangedEvents)
            {
                SegmentStateChanged?.Invoke(this, new SegmentStateChangedEventArgs(segmentIndex, isOn));
            }
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
                        _dirtyFlags[i] = true;
                    }
                }

                if (changed.Count > 0)
                {
                    Version++;
                }
            }

            if (RaiseSegmentStateChangedEvents && changed.Count > 0)
            {
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
