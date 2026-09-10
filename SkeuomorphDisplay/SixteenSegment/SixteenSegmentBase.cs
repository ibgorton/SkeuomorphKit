using System;
using System.Collections.Generic;
using System.Windows;

namespace SkeuomorphDisplay.SixteenSegment
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
        private const int SegmentCount = 16;
        private readonly bool[] _bits = new bool[SegmentCount];

        private static readonly Dictionary<char, bool[]> CharacterMap = new()
        {
            { ' ', new bool[SegmentCount] },
            { '-', new bool[SegmentCount] { false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false } },
            { '0', new bool[SegmentCount] { true, true, true, true, true, true, false, false, false, false, true, true, true, true, true, true } },
            { '1', new bool[SegmentCount] { false, false, false, false, false, true, true, false, false, false, false, false, false, false, true, true } },
            { '2', new bool[SegmentCount] { true, true, true, true, false, true, true, false, true, true, false, true, true, true, true, true } },
            { '3', new bool[SegmentCount] { true, true, true, true, false, true, true, false, false, true, true, true, true, true, true, true } },
            { '4', new bool[SegmentCount] { false, false, true, true, true, true, true, false, false, false, false, true, true, true, true, false } },
            { '5', new bool[SegmentCount] { true, true, true, true, true, false, true, false, false, true, true, true, true, true, true, false } },
            { '6', new bool[SegmentCount] { true, true, true, true, true, false, true, false, true, true, true, true, true, true, true, false } },
            { '7', new bool[SegmentCount] { true, true, true, false, false, true, true, false, false, false, false, true, true, false, false, true } },
            { '8', new bool[SegmentCount] { true, true, true, true, true, true, true, false, true, true, true, true, true, true, true, true } },
            { '9', new bool[SegmentCount] { true, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true } },
            { 'A', new bool[SegmentCount] { true, true, true, true, true, true, true, false, true, true, true, true, true, true, true, true } },
            { 'B', new bool[SegmentCount] { true, true, true, true, true, true, true, false, true, true, true, true, true, true, true, true } },
            { 'C', new bool[SegmentCount] { true, true, true, true, true, false, false, false, true, true, true, true, true, false, false, true } },
            { 'D', new bool[SegmentCount] { true, true, true, true, true, true, true, false, true, true, true, true, true, true, true, true } },
            { 'E', new bool[SegmentCount] { true, true, true, true, true, false, true, false, true, true, true, true, true, false, true, true } },
            { 'F', new bool[SegmentCount] { true, true, true, true, true, false, true, false, true, true, true, true, true, false, false, false } }
        };

        protected SixteenSegmentBase()
        {
        }

        public override void BlankModule()
        {
            var bits = new bool[SegmentCount];
            ApplyBits(bits);
        }

        public override void SetChar(char character)
        {
            char normalized = char.ToUpperInvariant(character);
            bool[] bits = CharacterMap.TryGetValue(normalized, out bool[]? mapped)
                ? mapped ?? CharacterMap[' ']
                : CharacterMap[' '];
            ApplyBits(bits);
        }

        private void ApplyBits(bool[] source)
        {
            Segment1On = source[0];
            Segment2On = source[1];
            Segment3On = source[2];
            Segment4On = source[3];
            Segment5On = source[4];
            Segment6On = source[5];
            Segment7On = source[6];
            Segment8On = source[7];
            Segment9On = source[8];
            Segment10On = source[9];
            Segment11On = source[10];
            Segment12On = source[11];
            Segment13On = source[12];
            Segment14On = source[13];
            Segment15On = source[14];
            Segment16On = source[15];
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
