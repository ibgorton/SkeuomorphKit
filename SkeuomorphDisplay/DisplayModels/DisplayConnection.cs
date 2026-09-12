using System;

namespace SkeuomorphDisplay
{
    public interface ISegmentDisplayHost
    {
        void ApplySegmentState(int segmentIndex, bool isOn);
        void Refresh();
    }

    public sealed class SegmentDisplayConnection<TDisplay> where TDisplay : SegmentDisplayState
    {
        private readonly TDisplay _display;
        private ISegmentDisplayHost? _host;
        private EventHandler<SegmentStateChangedEventArgs>? _boundHandler;

        public SegmentDisplayConnection(TDisplay display)
        {
            _display = display ?? throw new ArgumentNullException(nameof(display));
        }

        public TDisplay Display => _display;

        public int SegmentCount => _display.SegmentCount;

        public bool IsBound => _host is not null;

        public bool this[int segmentIndex]
        {
            get => _display.GetSegmentState(segmentIndex);
            set => _display.SetSegmentState(segmentIndex, value);
        }

        public void SetSegmentState(int segmentIndex, bool isOn) => _display.SetSegmentState(segmentIndex, isOn);

        public void ApplyBitPattern(bool[] pattern) => _display.ApplyBitPattern(pattern);

        public void ApplyBitPattern(ReadOnlySpan<bool> pattern) => _display.ApplyBitPattern(pattern);

        public void Blank() => _display.BlankModule();

        public void SetChar(char character) => _display.SetChar(character);

        public int[] DrainDirtySegments() => _display.DrainDirtySegments();

        public SegmentDisplayUpdate[] GetDirtyUpdates() => _display.GetDirtyUpdates();

        public bool HasDirtySegments => _display.HasDirtySegments;

        public int Version => _display.Version;

        public void Bind(ISegmentDisplayHost host)
        {
            if (host is null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (ReferenceEquals(_host, host) && _boundHandler is not null)
            {
                return;
            }

            Unbind();

            _host = host;
            _boundHandler = (_, args) =>
            {
                host.ApplySegmentState(args.SegmentIndex, args.IsOn);
                host.Refresh();
            };

            _display.RaiseSegmentStateChangedEvents = true;
            _display.SegmentStateChanged += _boundHandler;
        }

        public void Unbind()
        {
            if (_host is null || _boundHandler is null)
            {
                _host = null;
                _boundHandler = null;
                return;
            }

            _display.SegmentStateChanged -= _boundHandler;
            _host = null;
            _boundHandler = null;
        }
    }

    public static class DisplayFactory
    {
        public static SegmentDisplayConnection<SevenSegmentDisplay> CreateSevenSegment()
            => new(new SevenSegmentDisplay());

        public static SegmentDisplayConnection<Rectangle5x7Display> CreateRectangle5x7()
            => new(new Rectangle5x7Display());

        public static SegmentDisplayConnection<SixteenSegmentDisplay> CreateSixteenSegment()
            => new(new SixteenSegmentDisplay());

        public static SegmentDisplayConnection<TDisplay> Create<TDisplay>() where TDisplay : SegmentDisplayState, new()
            => new(new TDisplay());
    }
}
