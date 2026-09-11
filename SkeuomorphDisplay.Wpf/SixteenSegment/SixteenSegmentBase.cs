using System;
using System.Windows;

using SkeuomorphCore;

namespace SkeuomorphDisplay.Wpf.SixteenSegment
{
    /*              SEGMENT NUMBERING
    *         _____________   _____________
    *        /     ONE     | |     TWO     \ 
    *      __\_____________|_|_____________/__
    *     /   \  \        |   |        /  /   \
    *     |   |   \       |   |       /   |   |
    *     |   |\   \      |   |      /   /|   |
    *     | E | \ N \     | T |     / N / | T |
    *     | I |  \ I \    | E |    / E /  | H |
    *     | G |   \ N \   | N |   / V /   | R |
    *     | H |    \ E \  |   |  / E /    | E |
    *     | T |     \   \ |   | / L /     | E |
    *     |   |      \   \|   |/ E /      |   |
    *     |   |_______\___|___|___/_______|   |
    *     \___/  SIXTEEN   | |   TWELVE   \___/
    *     /   \____________|_|____________/   \
    *     |   |       /   |   |   \       |   |
    *     |   |      / N /| F |\ T \      |   |
    *     | S |     / E / | O | \ H \     | F |
    *     | E |    / E /  | U |  \ I \    | O |
    *     | V |   / T /   | R |   \ R \   | U |
    *     | E |  / F /    | T |    \ T \  | R | 
    *     | N | / I /     | E |     \ E \ |   |
    *     |   |/ F /      | E |      \ E \|   |
    *     |   |   /       | N |       \ N |   |
    *     \___/__/________|___|________\__\___/  
    *        /       SIX   | |  FIVE       \     /--\  <- DECIMAL POINT
    *        \_____________| |_____________/     \--/
    */

    public abstract class SixteenSegmentBase : DisplayControlBase
    {
        private readonly SixteenSegmentDisplay _display = new();
        private readonly SegmentValueMapper _segmentMapper;

        protected SixteenSegmentBase()
        {
            _display.RaiseSegmentStateChangedEvents = true;

            _segmentMapper = new SegmentValueMapper(
                value => Segment1On = value,
                value => Segment2On = value,
                value => Segment3On = value,
                value => Segment4On = value,
                value => Segment5On = value,
                value => Segment6On = value,
                value => Segment7On = value,
                value => Segment8On = value,
                value => Segment9On = value,
                value => Segment10On = value,
                value => Segment11On = value,
                value => Segment12On = value,
                value => Segment13On = value,
                value => Segment14On = value,
                value => Segment15On = value,
                value => Segment16On = value);

            _display.SegmentStateChanged += (_, e) => _segmentMapper.Apply(e.SegmentIndex, e.IsOn);
        }

        public bool ShowDecimalPoint
        {
            get => _display.ShowDecimalPoint;
            set => _display.ShowDecimalPoint = value;
        }

        public override void BlankModule()
        {
            _display.BlankModule();
        }

        public override void SetChar(char character)
        {
            _display.SetChar(character);
        }

        private static readonly DependencyProperty Segment1OnProperty = DependencyProperty.Register(nameof(Segment1On), typeof(bool), typeof(SixteenSegmentBase), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment2OnProperty = DependencyProperty.Register(nameof(Segment2On), typeof(bool), typeof(SixteenSegmentBase), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment3OnProperty = DependencyProperty.Register(nameof(Segment3On), typeof(bool), typeof(SixteenSegmentBase), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment4OnProperty = DependencyProperty.Register(nameof(Segment4On), typeof(bool), typeof(SixteenSegmentBase), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment5OnProperty = DependencyProperty.Register(nameof(Segment5On), typeof(bool), typeof(SixteenSegmentBase), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment6OnProperty = DependencyProperty.Register(nameof(Segment6On), typeof(bool), typeof(SixteenSegmentBase), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment7OnProperty = DependencyProperty.Register(nameof(Segment7On), typeof(bool), typeof(SixteenSegmentBase), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment8OnProperty = DependencyProperty.Register(nameof(Segment8On), typeof(bool), typeof(SixteenSegmentBase), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment9OnProperty = DependencyProperty.Register(nameof(Segment9On), typeof(bool), typeof(SixteenSegmentBase), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment10OnProperty = DependencyProperty.Register(nameof(Segment10On), typeof(bool), typeof(SixteenSegmentBase), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment11OnProperty = DependencyProperty.Register(nameof(Segment11On), typeof(bool), typeof(SixteenSegmentBase), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment12OnProperty = DependencyProperty.Register(nameof(Segment12On), typeof(bool), typeof(SixteenSegmentBase), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment13OnProperty = DependencyProperty.Register(nameof(Segment13On), typeof(bool), typeof(SixteenSegmentBase), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment14OnProperty = DependencyProperty.Register(nameof(Segment14On), typeof(bool), typeof(SixteenSegmentBase), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment15OnProperty = DependencyProperty.Register(nameof(Segment15On), typeof(bool), typeof(SixteenSegmentBase), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment16OnProperty = DependencyProperty.Register(nameof(Segment16On), typeof(bool), typeof(SixteenSegmentBase), new PropertyMetadata(false));

        public bool Segment1On { get => (bool)GetValue(Segment1OnProperty); set => SetValue(Segment1OnProperty, value); }
        public bool Segment2On { get => (bool)GetValue(Segment2OnProperty); set => SetValue(Segment2OnProperty, value); }
        public bool Segment3On { get => (bool)GetValue(Segment3OnProperty); set => SetValue(Segment3OnProperty, value); }
        public bool Segment4On { get => (bool)GetValue(Segment4OnProperty); set => SetValue(Segment4OnProperty, value); }
        public bool Segment5On { get => (bool)GetValue(Segment5OnProperty); set => SetValue(Segment5OnProperty, value); }
        public bool Segment6On { get => (bool)GetValue(Segment6OnProperty); set => SetValue(Segment6OnProperty, value); }
        public bool Segment7On { get => (bool)GetValue(Segment7OnProperty); set => SetValue(Segment7OnProperty, value); }
        public bool Segment8On { get => (bool)GetValue(Segment8OnProperty); set => SetValue(Segment8OnProperty, value); }
        public bool Segment9On { get => (bool)GetValue(Segment9OnProperty); set => SetValue(Segment9OnProperty, value); }
        public bool Segment10On { get => (bool)GetValue(Segment10OnProperty); set => SetValue(Segment10OnProperty, value); }
        public bool Segment11On { get => (bool)GetValue(Segment11OnProperty); set => SetValue(Segment11OnProperty, value); }
        public bool Segment12On { get => (bool)GetValue(Segment12OnProperty); set => SetValue(Segment12OnProperty, value); }
        public bool Segment13On { get => (bool)GetValue(Segment13OnProperty); set => SetValue(Segment13OnProperty, value); }
        public bool Segment14On { get => (bool)GetValue(Segment14OnProperty); set => SetValue(Segment14OnProperty, value); }
        public bool Segment15On { get => (bool)GetValue(Segment15OnProperty); set => SetValue(Segment15OnProperty, value); }
        public bool Segment16On { get => (bool)GetValue(Segment16OnProperty); set => SetValue(Segment16OnProperty, value); }
    }
}
