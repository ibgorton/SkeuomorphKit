using System;
using System.Windows;

using SkeuomorphCore;

namespace SkeuomorphDisplay.Wpf.Rectangle5x7
{
    /*
     * 
     *  [01] [02] [03] [04] [05]
     *  [06] [07] [08] [09] [10]
     *  [11] [12] [13] [14] [15]
     *  [16] [17] [18] [19] [20]
     *  [21] [22] [23] [24] [25]
     *  [26] [27] [28] [29] [30]
     *  [31] [32] [33] [34] [35]
     * 
     */

    public abstract class Rectangle5x7Base : DisplayControlBase
    {
        private readonly Rectangle5x7Display _display = new();
        private readonly SegmentValueMapper _segmentMapper;

        public Rectangle5x7Base()
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
                value => Segment16On = value,
                value => Segment17On = value,
                value => Segment18On = value,
                value => Segment19On = value,
                value => Segment20On = value,
                value => Segment21On = value,
                value => Segment22On = value,
                value => Segment23On = value,
                value => Segment24On = value,
                value => Segment25On = value,
                value => Segment26On = value,
                value => Segment27On = value,
                value => Segment28On = value,
                value => Segment29On = value,
                value => Segment30On = value,
                value => Segment31On = value,
                value => Segment32On = value,
                value => Segment33On = value,
                value => Segment34On = value,
                value => Segment35On = value);

            _display.SegmentStateChanged += (_, e) => _segmentMapper.Apply(e.SegmentIndex, e.IsOn);
        }

        public override void BlankModule()
        {
            _display.BlankModule();
        }

        public override void SetChar(char character)
        {
            _display.SetChar(character);
        }

        private static readonly DependencyProperty Segment1OnProperty = DependencyProperty.Register(nameof(Segment1On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment2OnProperty = DependencyProperty.Register(nameof(Segment2On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment3OnProperty = DependencyProperty.Register(nameof(Segment3On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment4OnProperty = DependencyProperty.Register(nameof(Segment4On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment5OnProperty = DependencyProperty.Register(nameof(Segment5On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment6OnProperty = DependencyProperty.Register(nameof(Segment6On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment7OnProperty = DependencyProperty.Register(nameof(Segment7On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment8OnProperty = DependencyProperty.Register(nameof(Segment8On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment9OnProperty = DependencyProperty.Register(nameof(Segment9On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment10OnProperty = DependencyProperty.Register(nameof(Segment10On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment11OnProperty = DependencyProperty.Register(nameof(Segment11On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment12OnProperty = DependencyProperty.Register(nameof(Segment12On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment13OnProperty = DependencyProperty.Register(nameof(Segment13On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment14OnProperty = DependencyProperty.Register(nameof(Segment14On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment15OnProperty = DependencyProperty.Register(nameof(Segment15On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment16OnProperty = DependencyProperty.Register(nameof(Segment16On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment17OnProperty = DependencyProperty.Register(nameof(Segment17On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment18OnProperty = DependencyProperty.Register(nameof(Segment18On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment19OnProperty = DependencyProperty.Register(nameof(Segment19On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment20OnProperty = DependencyProperty.Register(nameof(Segment20On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment21OnProperty = DependencyProperty.Register(nameof(Segment21On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment22OnProperty = DependencyProperty.Register(nameof(Segment22On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment23OnProperty = DependencyProperty.Register(nameof(Segment23On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment24OnProperty = DependencyProperty.Register(nameof(Segment24On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment25OnProperty = DependencyProperty.Register(nameof(Segment25On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment26OnProperty = DependencyProperty.Register(nameof(Segment26On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment27OnProperty = DependencyProperty.Register(nameof(Segment27On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment28OnProperty = DependencyProperty.Register(nameof(Segment28On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment29OnProperty = DependencyProperty.Register(nameof(Segment29On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment30OnProperty = DependencyProperty.Register(nameof(Segment30On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment31OnProperty = DependencyProperty.Register(nameof(Segment31On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment32OnProperty = DependencyProperty.Register(nameof(Segment32On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment33OnProperty = DependencyProperty.Register(nameof(Segment33On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment34OnProperty = DependencyProperty.Register(nameof(Segment34On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));
        private static readonly DependencyProperty Segment35OnProperty = DependencyProperty.Register(nameof(Segment35On), typeof(bool), typeof(Rectangle5x7Base), new PropertyMetadata(false));

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
        public bool Segment17On { get => (bool)GetValue(Segment17OnProperty); set => SetValue(Segment17OnProperty, value); }
        public bool Segment18On { get => (bool)GetValue(Segment18OnProperty); set => SetValue(Segment18OnProperty, value); }
        public bool Segment19On { get => (bool)GetValue(Segment19OnProperty); set => SetValue(Segment19OnProperty, value); }
        public bool Segment20On { get => (bool)GetValue(Segment20OnProperty); set => SetValue(Segment20OnProperty, value); }
        public bool Segment21On { get => (bool)GetValue(Segment21OnProperty); set => SetValue(Segment21OnProperty, value); }
        public bool Segment22On { get => (bool)GetValue(Segment22OnProperty); set => SetValue(Segment22OnProperty, value); }
        public bool Segment23On { get => (bool)GetValue(Segment23OnProperty); set => SetValue(Segment23OnProperty, value); }
        public bool Segment24On { get => (bool)GetValue(Segment24OnProperty); set => SetValue(Segment24OnProperty, value); }
        public bool Segment25On { get => (bool)GetValue(Segment25OnProperty); set => SetValue(Segment25OnProperty, value); }
        public bool Segment26On { get => (bool)GetValue(Segment26OnProperty); set => SetValue(Segment26OnProperty, value); }
        public bool Segment27On { get => (bool)GetValue(Segment27OnProperty); set => SetValue(Segment27OnProperty, value); }
        public bool Segment28On { get => (bool)GetValue(Segment28OnProperty); set => SetValue(Segment28OnProperty, value); }
        public bool Segment29On { get => (bool)GetValue(Segment29OnProperty); set => SetValue(Segment29OnProperty, value); }
        public bool Segment30On { get => (bool)GetValue(Segment30OnProperty); set => SetValue(Segment30OnProperty, value); }
        public bool Segment31On { get => (bool)GetValue(Segment31OnProperty); set => SetValue(Segment31OnProperty, value); }
        public bool Segment32On { get => (bool)GetValue(Segment32OnProperty); set => SetValue(Segment32OnProperty, value); }
        public bool Segment33On { get => (bool)GetValue(Segment33OnProperty); set => SetValue(Segment33OnProperty, value); }
        public bool Segment34On { get => (bool)GetValue(Segment34OnProperty); set => SetValue(Segment34OnProperty, value); }
        public bool Segment35On { get => (bool)GetValue(Segment35OnProperty); set => SetValue(Segment35OnProperty, value); }
    }
}
